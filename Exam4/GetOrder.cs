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
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "GetOrder")] HttpRequest req,
            ILogger log)
        {
            try
            {

               
                string customSearch = req.Query["customSearch"];
                string status = req.Query["statustype"];
                string isActive = req.Query["isActive"];
                string Product = req.Query["Product"];
                string startDateFilter = req.Query["startDate"];
                string endDateFilter = req.Query["endDate"];

                string requestData = await new StreamReader(req.Body).ReadToEndAsync();
                //var data = JsonConvert.DeserializeObject<filteroptions>(requestData);
                dynamic Data = JsonConvert.DeserializeObject(requestData);
                customSearch = customSearch ?? Data?.customSearch;
                status = status ?? Data?.status;
                isActive = isActive?? Data?.isActive;
                Product = Product ?? Data?.Product;
                startDateFilter = startDateFilter ?? Data?.startDate;
                endDateFilter = endDateFilter ?? Data?.endDate;

                DateTime startDate;
                DateTime endDate;

                if (!string.IsNullOrEmpty(startDateFilter) && !string.IsNullOrEmpty(endDateFilter))
                {
                    startDate = DateTime.Parse(startDateFilter);
                    endDate = DateTime.Parse(endDateFilter);
                }
                else
                {
                    DateTime today = DateTime.Today;
                    if (today.Day < 5)
                    {
                        startDate = today.AddMonths(-1);
                        endDate = today.AddDays(-1);
                    }
                    else
                    {
                        startDate = new DateTime(today.Year, today.Month, 1);
                        endDate = today.AddDays(1);
                    }
                }





                var query = _context.orderAddresses
                       .Include(o => o.Orders)
                       .ThenInclude(oi => oi.OrderItems).ThenInclude(a => a.product)
                       .Include(o => o.address)
                       .Where(o => o.Orders.OrderDate >= startDate && o.Orders.OrderDate <= endDate).ToList();

                query = query.Where(a => a.address.addresstype == addresstype.Shipping).ToList();

                if (Product != null)
                {
                    query = query.Where(o => o.Orders.OrderItems.Any(oi => oi.product.Name.ToLower() == Product.ToLower() )).ToList();
                }
                if (status != null)
                {
                    query = (List<orderAddress>)query.Where(o => o.Orders.StatusType.ToString().ToLower() == status.ToLower()).ToList();

                }
                if (!string.IsNullOrEmpty(customSearch))
                {
                    query = _context.orderAddresses.Where(u => u.Orders.CustomerName.ToLower() == customSearch.ToLower() || u.Orders.CustomerEmail.ToLower().Contains(customSearch.ToLower())).ToList();

                }
                if (isActive != null)
                {
                    query = _context.orderAddresses.Where(u => u.Orders.IsActive.ToString().ToLower() == isActive.ToLower()).ToList();

                }



                List<OrderRes> OrderResponses = query.Select(oa => new OrderRes
                  {
                       OrderId = oa.OrderId,
                     Description = oa.Orders.Note,
                      CustomerName = oa.Orders.CustomerName,
                      CustomerEmail = oa.Orders.CustomerEmail,
                     Status = oa.Orders.StatusType.ToString(),
                    TotalItems = oa.Orders.OrderItems.Count,
                       TotalAmount = oa.Orders.TotalAmount,
                       ShippingAddress = $"{oa.address.Address}, {oa.address.city}, {oa.address.state}, {oa.address.country}-{oa.address.zipcode}"
                    }).ToList();
                return new OkObjectResult(OrderResponses);
            }
            catch (Exception ex)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

        }
    }
}
    
