using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using pracical1.dataAccess;
using Microsoft.EntityFrameworkCore;

namespace Exam4
{
    public  class product
    {

        private readonly ApplicationDbContext _context;
        public product(ApplicationDbContext context)
        {
            _context = context;
        }
        [FunctionName("product")]
        public  async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get",  Route = "Product")] HttpRequest req
           )
        {
            var product = await _context.products.ToListAsync();

            return new OkObjectResult(product);
        }
    }
}
