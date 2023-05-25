using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Build.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace practical1.Models
{
    public class orderAddress
    {
        public int orderAddressId { get; set; }
        [Required]
        public int OrderId { get; set; }
        [ForeignKey("OrderId")]
        [ValidateNever]
        public Order Orders { get; set; }

        public int AddressId { get; set; }
        [ForeignKey("AddressId")]
        [ValidateNever]
        public address address { get; set; }
    }
}
