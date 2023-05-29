using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using practical1.Models.Models;
using practical1.Models;
using pracical1.dataAccess;
using practical1.Models.ViewModel;
using System.Collections.Generic;

namespace Exam4
{
    public  class Draftorder
    {
        private readonly ApplicationDbContext _context;

        public Draftorder(ApplicationDbContext context)
        {
            _context = context;
        }
        [FunctionName("Draftorder")]
        public  async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous,"post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var orders = JsonConvert.DeserializeObject<OrderVm>(requestBody);

                var order = new Order
                {
                    OrderDate = DateTime.Now,
                    Note = orders.Note,
                    StatusType = Status.Draft,
                    DisountAmount = orders.DisountAmount,
                    CustomerName = orders.CustomerName,
                    CustomerEmail = orders.CustomerEmail,
                    CustomerContactNo = orders.CustomerContactNo,
                    IsActive = true,
                    CreatedOn = DateTime.Now
                };

                var orderItems = new List<OrderItem>();
                decimal totalOrderPrice = 0;

                foreach (var item in orders.orderItems)
                {
                    var product = await _context.products.FirstOrDefaultAsync(x => x.productId == item.ProductId);
                    if (product == null)
                    {
                        return new BadRequestObjectResult($"Product with id {item.ProductId} not found");
                    }
                    if (product.Quantity < item.Quantity)
                    {
                        return new BadRequestObjectResult($"Product with id {item.ProductId} does not have enough quantity");
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
                    _context.products.Update(product);
                }

                order.OrderItems = orderItems;
                order.TotalAmount = (double)totalOrderPrice;

                if (orders.DisountAmount < 0)
                {
                    return new BadRequestObjectResult("Discount cannot be negative");
                }

                if (orders.DisountAmount > 0)
                {
                    double discountPercentage = orders.DisountAmount / 100.0;
                    double discountAmount = (double)totalOrderPrice * discountPercentage;
                    order.TotalAmount = (double)totalOrderPrice - discountAmount;
                    order.DisountAmount = orders.DisountAmount;
                }

                _context.orders.Add(order);

               
                var address = await _context.addresses.FirstOrDefaultAsync(a => a.AddressId == orders.AddressId);

                if (address == null)
                {
                    return new NotFoundObjectResult($"Address with ID {orders.AddressId} not found");
                }

             
                var orderAddress = new orderAddress
                {
                    Orders = order,
                    address = address,
                };

                _context.orderAddresses.Add(orderAddress);

                await _context.SaveChangesAsync();

                return new OkResult();
            }
            catch (Exception ex)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
    }

