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
                var Req = JsonConvert.DeserializeObject<orderAddress>(requestBody);
                orderAddress OrderAddress = await _context.orderAddresses.FindAsync(id);
                if (OrderAddress == null)
                {
                    return new NotFoundObjectResult($"OrderAddress with orderAddressId {id} not found");
                }

                var order = await _context.orders.FindAsync(Req.OrderId);
                if(order == null){
                    return new NotFoundObjectResult($"order with id {Req.OrderId} not found");
                }

                var address = await _context.addresses.FindAsync(Req.AddressId);
                if (address == null)
                {
                    return new NotFoundObjectResult($"address with id {Req.AddressId} not found");
                }

                // Update the orderId and addressId
                OrderAddress.OrderId = Req.OrderId; ;
                OrderAddress.AddressId = Req.AddressId;

                 _context.orderAddresses.Update(OrderAddress);
                await _context.SaveChangesAsync();

                return new OkObjectResult("OrderAddress updated successfully!");
            }
            catch (Exception ex)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

        }
    }
}
