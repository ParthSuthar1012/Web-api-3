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
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "UpdateOrderStatus")] HttpRequest req
     
           )
        {
            
                try
                {

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject(requestBody);

                string orderId = data.orderId;
                string statusType = data.statusType;

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

                if (order.IsActive == false)
                {
                    return new BadRequestObjectResult($"Order with id {order.OrderId} is inactive. Cannot change");
                }

                if (order.StatusType == Status.Paid)
                {
                    return new BadRequestObjectResult($"Order with id {order.OrderId} already paid. Cannot change the status to {statusType}");
                }

                order.StatusType = parsedStatusType;

                SaveOrder(order);

                    return new OkObjectResult(order);
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


