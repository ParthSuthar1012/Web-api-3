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
    public  class Addadress
    {
        private readonly ApplicationDbContext _context;
        public Addadress(ApplicationDbContext context)
        {
            _context = context;
        }
        [FunctionName("Addadress")]
        public  async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "Address")] HttpRequest req
           )
        {
            if(req.Method == HttpMethods.Post)
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var address = JsonConvert.DeserializeObject<address>(requestBody);
                _context.addresses.Add(address);
                _context.SaveChanges();
                return new CreatedResult("/address", address);
            }
            var addresses = await _context.addresses.ToListAsync();


            var convertedAddresses = addresses.Select(a => new
            {
                addressId = a.AddressId,
                addresstype = a.addresstype.ToString(),
                address = a.Address,
                country = a.country,
                city = a.city,
                state = a.state,
                zipcode = a.zipcode,
                contactPerson = a.contactPerson,
                contectNo = a.contectNo
            }).ToList();


            return new OkObjectResult(convertedAddresses);
        }
    }
}
