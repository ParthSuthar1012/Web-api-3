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
                // Calculate the default date range
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
                     .ThenInclude(oi => oi.OrderItems)
                     .ThenInclude(oip => oip.product)
                     .Include(o => o.address);
                     //.Where(o => o.Orders.OrderDate >= filterOptions.StartDate && o.Orders.OrderDate <= filterOptions.EndDate);

                // Apply filters based on the provided filter options
                //if (!string.IsNullOrEmpty(filterOptions.CustomerName))
                //    query = query.Where(o => o.Orders.CustomerName.Contains(filterOptions.CustomerName));

                //if (!string.IsNullOrEmpty(filterOptions.CustomerEmail))
                //    query = query.Where(o => o.Orders.CustomerEmail.Contains(filterOptions.CustomerEmail));

                //if (filterOptions.IsActive == null)
                //    query = query.Where(o => o.Orders.IsActive == filterOptions.IsActive);

                //if (filterOptions.Status == null)
                //    query = query.Where(o => o.Orders.StatusType == filterOptions.Status);

                //if (filterOptions.ProductIds != null && filterOptions.ProductIds.Any())
                //    query = query.Where(o => o.Orders.OrderItems.Any(oi => filterOptions.ProductIds.Contains(oi.ProductId)));

                // Retrieve the orders from the database
                var order = await query.ToListAsync();

                // Map the orders to the response model
                List<OrderRes> orderResponses = order.Select(oa => new OrderRes
                {
                    OrderId = oa.OrderId,
                    Description = oa.Orders.OrderItems.,
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
    
