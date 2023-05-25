using Microsoft.Build.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace practical1.Models
{
    public class address
    {
        public int AddressId {  get; set; }
        [Required]
        public addresstype addresstype { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string country { get; set; }
        [Required]
        public string city { get; set; }
        [Required]
        public string state { get; set; }
        [Required]
        public string zipcode { get; set; }
        [Required]
        public string contactPerson { get; set; }
        [Required]
        public string contectNo { get; set; }
    }
}
