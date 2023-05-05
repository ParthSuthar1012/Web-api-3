using MessagePack;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;


namespace practical1.Models
{
    public class Order
    {
        
        public int OrderId { get; set; }
        [Required]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Required]
        public string Note { get; set; }
        [Required]
        public double DisountAmount { get; set; }

        [Required]
        public Status  StatusType { get; set; }
        [Required]
        public double TotalAmount { get; set; }
        [Required]
        public string CustomerName { get; set; }
        [Required]
        public string CustomerEmail { get; set; }
        [Required]
        public string CustomerContactNo { get; set; }
        [Required]
        public bool IsActive { get; set; }
        [Required]
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        [Required]
        public DateTime ModifiedOn { get; set; }

        public List<OrderItem> OrderItems { get; set; }


    }
}
