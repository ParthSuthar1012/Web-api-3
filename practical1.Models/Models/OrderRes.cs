using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace practical1.Models.Models
{
    public class OrderRes
    {
        public int OrderId { get; set; }
 
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public int TotalItems { get; set; }
        public double TotalAmount { get; set; }
        public string ShippingAddress { get; set; }
    }
}
