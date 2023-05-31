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
using practical1.Models;
using pracical1.dataAccess;
using practical1.Models.ViewModel;

namespace Exam4
{
    public  class updateOrderAddress
    {

        private readonly ApplicationDbContext _context;

        public updateOrderAddress(ApplicationDbContext context)
        {
            _context = context;
        }
        [FunctionName("updateOrderAddress")]
        public  async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "updateOrderAddress/{id}")] HttpRequest req,
            int id)
        {
            try
            {

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject(requestBody);
                int AddressId = data.AddressId;

                var address = await _context.addresses.FirstOrDefaultAsync(a => a.AddressId == AddressId);
                if (address.addresstype != addresstype.Shipping )
                {
                    return new BadRequestObjectResult($"Adderss With Id {AddressId} is Not Shiiping Address");
                }

                var order = await _context.orders.FirstOrDefaultAsync(a => a.OrderId == id);
          
                orderAddress OrderAddress = await _context.orderAddresses.FirstOrDefaultAsync(a => a.OrderId == id);

          
                if (OrderAddress == null)
                {
                    return new NotFoundObjectResult($"OrderAddress with Order Id {id} not found");
                }

                
                if(order == null){
                    return new NotFoundObjectResult($"order with id {order.OrderId} not found");
                }
                if (order.StatusType == Status.Shipped )
                {
                    return new BadRequestObjectResult("Order is Shhipped Can not chnage address");
                }

                if (address == null)
                {
                    return new NotFoundObjectResult($"address with id {AddressId} not found");
                }

            
                OrderAddress.AddressId = data.AddressId;

                 _context.orderAddresses.Update(OrderAddress);
                await _context.SaveChangesAsync();

                return new OkObjectResult(OrderAddress);
            }
            catch (Exception ex)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

        }
    }
}
