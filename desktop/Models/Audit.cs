using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace desktop.Models
{
    public class Audit
    {
        public DateTime Date { get; set; }
        public string TimeEntry { get; set; }
        public int QuantityActivity { get; set; }
    }
}
