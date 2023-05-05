using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using practical1.Models;
using practical1.Models.ViewModel;
using Repository.Repository;
using Repository.Repository.IRepository;

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
        public async Task<ActionResult> Post(OrderVm orders)
        {
            // Add order to the repository
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
                ModifiedOn = DateTime.Now
            };

            _unitOfWork.orderRepository.Add(order);
            _unitOfWork.Save();

            // Add order items to the repository
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
            _unitOfWork.orderRepository.update(order);

            _unitOfWork.Save();

            return Ok($"Order with id {order.OrderId} added successfully");
        }



        //[HttpPost]
        //[Route("Add-Order")]
        //public async Task<ActionResult> Post(OrderVm orders)
        //{
        //    // Add order to the repository
        //    var order = new Order
        //    {
        //        OrderDate = DateTime.Now,
        //        Note = orders.Note,
        //        StatusType = Status.Open,
        //        CustomerName = orders.CustomerName,
        //        CustomerEmail = orders.CustomerEmail,
        //        CustomerContactNo = orders.CustomerContactNo,
        //        DisountAmount = orders.DisountAmount,
        //        IsActive = true,
        //        CreatedOn = DateTime.Now,
        //        ModifiedOn = DateTime.Now
        //    };

        //    _unitOfWork.orderRepository.Add(order);
        //    _unitOfWork.Save();

        //    // Add order items to the repository
        //    var orderItems = new List<OrderItem>();
        //    decimal totalOrderPrice = 0;

        //    foreach (var item in orders.orderItems)
        //    {
        //        var product = _unitOfWork.productRepository.GetFirstOrDefault(x => x.productId == item.ProductId);
        //        if (product == null)
        //        {
        //            return BadRequest($"Product with id not found");
        //        }
        //        if (product.Quantity < item.Quantity)
        //        {
        //            return BadRequest($"Product with id {item.ProductId} does not have enough quantity");
        //        }

        //        // Calculate price with discount
        //        decimal discountedPrice = (decimal)(item.Quantity * product.Price - order.DisountAmount);
        //        if (discountedPrice < 0) discountedPrice = 0; 

        //        var orderItem = new OrderItem
        //        {
        //            ProductId = item.ProductId,
        //            Quantity = item.Quantity,
        //            Price = (int)discountedPrice,
        //            IsActive = true
        //        };
        //        totalOrderPrice += orderItem.Price;
        //        orderItems.Add(orderItem);

        //        product.Quantity -= item.Quantity;
        //        _unitOfWork.productRepository.update(product);

        //    }
        //    if (orders.DisountAmount > 0)
        //    {
        //        double discountPercentage = orders.DiscountAmount / 100.0;
        //        double discountAmount = totalOrderPrice * discountPercentage;
        //        order.TotalAmount = (double)(totalOrderPrice - discountAmount);
        //        order.DiscountAmount = orders.DiscountAmount;
        //    }



        //    order.OrderItems = orderItems;
        //    order.TotalAmount = (double)totalOrderPrice;
        //    _unitOfWork.orderRepository.update(order);

        //    _unitOfWork.Save();

        //    return Ok($"Order with id {order.OrderId} added successfully");
        //}







        [HttpGet("{id}")]
      
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
        public IEnumerable<Order> Getall()
        {
            return  _unitOfWork.orderRepository.GetAll();
        }

        [HttpPut]
        [Route("UpdateOrder")]

        public IActionResult Put(Order order)
        {
            if (order == null && order.OrderId == null)
            {
                return BadRequest();
            }
            var orderData = _unitOfWork.orderRepository.GetFirstOrDefault(a => a.OrderId == order.OrderId);

            if (orderData == null)
            {
                return BadRequest();
            }

            _unitOfWork.orderRepository.update(order);
            _unitOfWork.Save();
            return Ok(orderData);

        }
    }
}
