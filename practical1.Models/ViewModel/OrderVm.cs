using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace practical1.Models.ViewModel
{
    public class OrderVm
    {
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerContactNo { get; set; }
        public string Note { get; set; }
        public double DisountAmount { get; set; }
        public ICollection<OrderItemVm> orderItems { get; set; }


        public int BillingAddressId { get; set; }
        public int ShippingAddresId { get; set; }
    }
}
