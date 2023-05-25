using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using practical1.Models;
using pracical1.dataAccess;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Exam4
{
    public  class UpdateOrderStatus
    {
        private readonly ApplicationDbContext _context;

        public UpdateOrderStatus(ApplicationDbContext context)
        {
            _context = context; 
        }
        [FunctionName("UpdateOrderStatus")]
        public  async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "Order/{orderId}/status/{statusType}")] HttpRequest req, string orderId,
        string statusType
           )
        {
            
                try
                {
                   
                    if (!int.TryParse(orderId, out int orderIdInt))
                    {
                        return new BadRequestObjectResult("Invalid orderId");
                    }

                    if (!Enum.TryParse(statusType, out Status parsedStatusType))
                    {
                        return new BadRequestObjectResult("Invalid statusType");
                    }

                    Order order = GetOrderById(orderIdInt);

                    if (order == null)
                    {
                        return new NotFoundObjectResult($"Order with ID {orderId} not found");
                    }

                    order.StatusType = parsedStatusType;

                SaveOrder(order);

                    return new OkObjectResult("Order status updated successfully!");
                }
                catch (Exception ex)
                {
                  
                    return new StatusCodeResult(StatusCodes.Status500InternalServerError);
                }
            }

            private  Order GetOrderById(int orderId)
            {
              var order = _context.orders.FirstOrDefault(a => a.OrderId == orderId);
            return order;
            }

            private  async Task SaveOrder(Order order)
            {
                _context.orders.Update(order);
            _context.SaveChangesAsync();
            }


        }
    }


