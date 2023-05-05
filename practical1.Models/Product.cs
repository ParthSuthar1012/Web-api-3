using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace practical1.Models
{
    public class Product
    {
        public int productId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        public bool IsActive { get; set; }
        [Required]
        public double Price { get; set; }
        [Required]
        public int Quantity { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; } = DateTime.Now;

        public DateTime ModifiedOn { get; set; }

        [ValidateNever]
        public string Imageurl { get; set; }
        [Required]
        public int CategoryId { get; set; }
        [ValidateNever]
        public Category Category { get; set; }
        public string? LatestData { get; set; }
    }
}
