using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using practical1.Models;
using practical1.Models.ViewModel;
using Repository.Repository;
using Repository.Repository.IRepository;
using System.Linq;

namespace Practical3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class OrderController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderVm OrderVm { get; set; }

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }



        [HttpPost]
        [Route("Add-Order")]
        public async Task<ActionResult> Addorder(OrderVm orders)
        {

            var order = new Order
            {
                OrderDate = DateTime.Now,
                Note = orders.Note,
                StatusType = Status.Open,
                CustomerName = orders.CustomerName,
                CustomerEmail = orders.CustomerEmail,
                CustomerContactNo = orders.CustomerContactNo,
                IsActive = true,
                CreatedOn = DateTime.Now,
                
            };

            //_unitOfWork.orderRepository.Add(order);
            //_unitOfWork.Save();


            var orderItems = new List<OrderItem>();
            decimal totalOrderPrice = 0;

            foreach (var item in orders.orderItems)
            {
                var product = _unitOfWork.productRepository.GetFirstOrDefault(x => x.productId == item.ProductId);
                if (product == null)
                {
                    return BadRequest($"Product with id not found");
                }
                if (product.Quantity < item.Quantity)
                {
                    return BadRequest($"Product with id {item.ProductId} does not have enough quantity");
                }
                var orderItem = new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = (int)(product.Price * item.Quantity),
                    IsActive = true
                };
                totalOrderPrice += orderItem.Price;
                orderItems.Add(orderItem);

                product.Quantity -= item.Quantity;
                _unitOfWork.productRepository.update(product);
                orderItems.Add(orderItem);
            }

            order.OrderItems = orderItems;
            order.TotalAmount = (double)totalOrderPrice;
            if (orders.DisountAmount < 0)
            {
                return BadRequest($"Discount Can't be in negative");
            }
            if (orders.DisountAmount > 0)
            {
                double discountPercentage = orders.DisountAmount / 100.0;
                double discountAmount = (double)totalOrderPrice * discountPercentage;
                order.TotalAmount = ((double)totalOrderPrice - discountAmount);
                order.DisountAmount = orders.DisountAmount;
            }
            _unitOfWork.orderRepository.Add(order);
            _unitOfWork.orderRepository.update(order);

            _unitOfWork.Save();

            return Ok($"Order with id {order.OrderId} added successfully");
        }


        [HttpGet]
        [Route("GetById")]
        [Authorize]
        public IActionResult GetBy(int id)
        {
            var oraderdata = _unitOfWork.orderRepository.GetFirstOrDefault(a => a.OrderId == id);
            if (oraderdata == null)
            {
                return NotFound();
            }
            return Ok(oraderdata);
        }

        [HttpGet]
        [Route("GetAllOrders")]
        [Authorize]
        public IActionResult GetAllOrders(DateTimeOffset? Date = null,  Status? statusType = null, string? customerSearch = null)
        {
            try
            {
                var orders = _unitOfWork.orderRepository.GetAll();
                // for Date Filter
                if (Date != null)
                {
                    orders = orders.Where(o => o.CreatedOn >= Date);
                }
           
                // For Status Type
                if (statusType != null)
                {
                    orders = orders.Where(o => o.StatusType == statusType.Value);
                    if (orders.Count() == 0)
                    {
                        return NotFound($"No orders found for Status '{statusType}'");
                    }
                }
                // For Customer Search
                if (!string.IsNullOrEmpty(customerSearch))
                {
                    var searchTerm = customerSearch.ToLower();
                    orders = orders.Where(o => o.CustomerName.ToLower().Contains(searchTerm) || o.CustomerEmail.ToLower().Contains(searchTerm));

                   if ( orders.Count() == 0)
                    {
                        return NotFound($"No orders found for customer search '{customerSearch}'");
                    }
                    
                }

                var orderVm = orders.Select(o => new
                {
                    OrderId = o.OrderId,
                    CreatedOn = o.CreatedOn,
                    StatusType = o.StatusType,
                    Note = o.Note
                }).ToList();

                return Ok(orderVm);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving orders: {ex.Message}");
            }
        }


        [HttpPut]
        [Route("UpdateOrderStatus/{orderId}/{statusType}")]
        [Authorize]
        public IActionResult UpdateOrderStatus(int orderId, string statusType)
        {
            var order = _unitOfWork.orderRepository.GetFirstOrDefault(o => o.OrderId == orderId);

            if (order == null)
            {
                return NotFound();
            }
            if (order.IsActive == false)
            {
                return BadRequest($"Order with id {order.OrderId} is inactive. Cannot change the status to {statusType}");
            }

            if (order.StatusType == Status.Paid)
            {
                return BadRequest($"Order with id {order.OrderId} already paid. Cannot change the status to {statusType}");
            }



            switch (statusType.ToLower())
            {
                case "open":
                    if (order.StatusType == Status.Shipped)
                    {
                        return BadRequest($"Order with id {order.OrderId} already shipped. Cannot change the status");
                    }
                    else
                    {
                        order.StatusType = Status.Open;
                    }
                    break;
                case "draft":
                    if (order.StatusType == Status.Shipped)
                    {
                        return BadRequest($"Order with id {order.OrderId} already shipped. Cannot change the status");
                    }
                    else
                    {
                        order.StatusType = Status.Draft;
                    }
                    break;
                case "shipped":
                    order.StatusType = Status.Shipped;

                    break;
                case "paid":
                    if (order.StatusType == Status.Shipped)
                    {
                        order.StatusType = Status.Paid;
                    }
                    else
                    {
                        return BadRequest($"Order with id {order.OrderId} must be shipped before changing the status to paid");
                    }
                    break;
                default:
                    return BadRequest("Invalid status type");
            }
            order.ModifiedOn = DateTime.Now;
            _unitOfWork.orderRepository.update(order);
            _unitOfWork.Save();

            return Ok($"Order status of id {order.OrderId} changed successfully");
        }

        [HttpPut]
        [Route("ToggleOrderStatus/{orderId}")]
        [Authorize]
        public IActionResult ToggleOrderStatus(int orderId)
        {
         
            var order = _unitOfWork.orderRepository.GetFirstOrDefault(o => o.OrderId == orderId);

            if (order == null)
            {
                return NotFound();
            }

            if (order.StatusType == Status.Shipped && order.IsActive == true)
            {
                return BadRequest($"Order with id {order.OrderId} already shipped. Cannot inactive the order");
            }

            if (order.StatusType == Status.Paid && order.IsActive == true)
            {
                return BadRequest($"Order with id {order.OrderId} already paid. Cannot inactive the order");
            }

            order.IsActive = !order.IsActive;
            order.ModifiedOn = DateTime.Now;

            _unitOfWork.orderRepository.update(order);
            _unitOfWork.Save();

            var status = order.IsActive ? "activated" : "inactivated";
            return Ok($"Order with id {order.OrderId} {status} successfully");
        }


        [HttpDelete]
        [Route("RemoveItem")]
        [Authorize]
        public IActionResult RemoveItem(int id, int itemId)
        {

            var order = _unitOfWork.orderRepository.GetFirstOrDefault(x => x.OrderId == id);

            if (order.StatusType == Status.Shipped || order.StatusType == Status.Paid  )
            {

                return BadRequest($"Order with id {order.OrderId} may be  shipped or paid. Cannot edit the order");
            }

            if ( order.IsActive == false)
            {
                return BadRequest($"Order With Id {order.OrderId} is InActive. Cannot edit the Order");
            }

            if (order == null)
            {
                return NotFound();
            }
            var Item = _unitOfWork.orderItemRepository.GetFirstOrDefault(x => x.OrderItemId == itemId);

            if (Item == null)
            {
                return NotFound();
            }
            var product = _unitOfWork.productRepository.GetFirstOrDefault(x => x.productId == Item.ProductId);
            if (product == null)
            {
                return BadRequest($"Product with id not found");
            }
            product.Quantity += Item.Quantity;
            
                double Percentage = order.DisountAmount / 100.0;
                double DiscountAmount = Item.Price * Percentage;
                order.TotalAmount -= (Item.Price - DiscountAmount);

            
            _unitOfWork.productRepository.update(product);
            if (Item == null) { return NotFound(); }
            _unitOfWork.orderItemRepository.Remove(Item);
            order.ModifiedOn = DateTime.Now;
            _unitOfWork.Save();
            if ( order.OrderItems.Count == 0)
            {
                _unitOfWork.orderRepository.Remove(order);
            }
            _unitOfWork.Save();
            return Ok("Order Item Deleted");

        }


        [HttpPut]
        [Route("UpdateItemQuantity")]
        [Authorize]
        public IActionResult UpdateOrderItemQuantity(int orderItemId, int newQuantity)
        {
            var orderItem = _unitOfWork.orderItemRepository.GetFirstOrDefault(u => u.OrderItemId == orderItemId);
            var order = _unitOfWork.orderRepository.GetFirstOrDefault(u => u.OrderId == orderItem.OrderId);

            if (order.StatusType == Status.Shipped || order.StatusType == Status.Paid)
            {

                return BadRequest($"Order with id {order.OrderId} may be  shipped or paid. Cannot edit the order");
            }

            if (order.IsActive == false)
            {
                return BadRequest($"Order With Id {order.OrderId} is InActive. Cannot edit the Order");
            }
            if (orderItem == null)
            {
                return NotFound($"Order item with id {orderItemId} not found");
            }

            var product = _unitOfWork.productRepository.GetFirstOrDefault(u => u.productId == orderItem.ProductId);

            if (product == null)
            {
                return BadRequest($"Product with id {orderItem.ProductId} not found");
            }
            if (newQuantity <= 0)
            {
                return BadRequest($"Invalid quantity: {newQuantity}");
            }
            if (orderItem.Quantity == newQuantity)
            {
                return BadRequest($"Product Has Same Quantity");
            }
            // for less Quantity then older Quantity
            if ( orderItem.Quantity > newQuantity) {
            
                var quantity = orderItem.Quantity - newQuantity;
                if (product.Quantity < newQuantity)
                {
                    return BadRequest($"Product with id {product.productId} does not have enough quantity");
                }
                product.Quantity += quantity;
                orderItem.Quantity = newQuantity;
                orderItem.Price = (int)product.Price * newQuantity;
                var changePrice = product.Price * quantity;
                var newPrice = orderItem.Price - (changePrice * (order.DisountAmount / 100));
                order.TotalAmount = order.TotalAmount - newPrice;

            }
            // For More Quantity then older Quantity
            if (orderItem.Quantity < newQuantity)
            {
                var quantity = newQuantity - orderItem.Quantity;
                if (product.Quantity < newQuantity)
                {
                    return BadRequest($"Product with id {product.productId} does not have enough quantity");
                }
                product.Quantity -= quantity;
                orderItem.Quantity = newQuantity;
                var changePrice = product.Price * quantity; 
                var newPrice = changePrice - (changePrice * (order.DisountAmount / 100));
                order.TotalAmount = order.TotalAmount + newPrice;
                orderItem.Price = (int)product.Price * newQuantity;

            }
         
            order.ModifiedOn = DateTime.Now;

            _unitOfWork.Save();

            return Ok($"Order item with id {orderItemId} quantity updated to {newQuantity}");
        }

    }
}
