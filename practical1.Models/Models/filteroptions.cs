using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace practical1.Models.Models
{
    public class filteroptions
    {
        public DateTime FirstDate { get; set; }
        public DateTime LastDate { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public bool IsActive { get; set; }
        public string status { get; set; }
        public List<int> ProductIds { get; set; }
    }
}
