
using NuGet.Protocol.Plugins;
using Org.BouncyCastle.Crypto.Engines;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace practical1.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public bool IsActive { get; set; }
        [Required]
        public DateTime CreatedOn { get; set; }

        public DateTime ModifiedOn { get; set; }
        public int? ParentCategoryId { get; set; }


    }
    public class Student
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}
