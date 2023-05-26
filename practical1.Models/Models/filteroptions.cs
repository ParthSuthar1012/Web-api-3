using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace practical1.Models.Models
{
    public class filteroptions
    {
          public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string CustomerSearch { get; set; }
  
    public bool IsActive { get; set; }
    public string Status { get; set; }
    public List<int> ProductIds { get; set; }
    }
}
