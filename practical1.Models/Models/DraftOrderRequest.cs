using practical1.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace practical1.Models.Models
{
    public class DraftOrderRequest
    {
        public string Note { get; set; }
        public double DiscountAmount { get; set; }
        public double TotalAmount { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerContactNo { get; set; }
        public ICollection<OrderItemVm> orderItems { get; set; }
        public int AddressId { get; set; } 

    }
}
