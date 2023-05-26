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
using System.Linq;
using practical1.Models.Models;
using System.Collections.Generic;

namespace Exam4
{
    public class GetOrder
    {
        private readonly ApplicationDbContext _context;

        public GetOrder(ApplicationDbContext context)
        {
            _context = context;
        }
        [FunctionName("GetOrder")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "GetOrder")] HttpRequest req,
            ILogger log)
        {
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var filterOptions = JsonConvert.DeserializeObject<filteroptions>(requestBody);
                
                DateTime startDate;
                DateTime endDate;

                DateTime today = DateTime.Today;
                if (today.Day < 5)
                {
                    startDate = today.AddMonths(-1);
                    endDate = today.AddDays(-1);
                }
                else
                {
                    startDate = new DateTime(today.Year, today.Month, 1);
                    endDate = today;
                }

                // Apply the date range filter
                if (filterOptions.StartDate == default)
                    filterOptions.StartDate = startDate;

                if (filterOptions.EndDate == default)
                    filterOptions.EndDate = endDate;

                var query = _context.orderAddresses
                     .Include(o => o.Orders)
                     .ThenInclude(oi => oi.OrderItems).ThenInclude(op => op.product)
                     .Include(o => o.address)
                     .Where(o => o.Orders.OrderDate >= filterOptions.StartDate && o.Orders.OrderDate <= filterOptions.EndDate);

                if (filterOptions.Status != null)
                    query = query.Where(o => o.Orders.StatusType.ToString() == filterOptions.Status);

                if (filterOptions.CustomerSearch != null)
                    query = query.Where(o => o.Orders.CustomerName.ToLower().Contains(filterOptions.CustomerSearch.ToLower()) || o.Orders.CustomerEmail.ToLower().Contains(filterOptions.CustomerSearch.ToLower()));

             

                var order = await query.ToListAsync();

                
                List<OrderRes> orderResponses = order.Select(oa => new OrderRes
                {
                    OrderId = oa.OrderId,
                    
                    CustomerName = oa.Orders.CustomerName,
                    CustomerEmail = oa.Orders.CustomerEmail,
                    Status = oa.Orders.StatusType.ToString(),
                    TotalItems = oa.Orders.OrderItems.Count,
                    TotalAmount = oa.Orders.TotalAmount,
                    ShippingAddress = $"{oa.address.Address}, {oa.address.city}, {oa.address.state}, {oa.address.country}-{oa.address.zipcode}"
                }).ToList();

                return new OkObjectResult(orderResponses);
            }
            catch (Exception ex)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

        }
    }
}
    
