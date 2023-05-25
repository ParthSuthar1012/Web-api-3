using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using pracical1.dataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: FunctionsStartup(typeof(Exam4.Statup))]
namespace Exam4
{
    public class Statup : FunctionsStartup
    {
       

        public override void Configure(IFunctionsHostBuilder builder)
        {
            var con = "Server=localhost;User=root;Password=;Database=parth_practical1;ConvertZeroDatetime=true;";
            builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseMySql(con, ServerVersion.AutoDetect(con)));




        }
    }
}
