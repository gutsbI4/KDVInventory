using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace desktop.Models
{
    public class OrderCollection
    {
        public IEnumerable<Order> Orders { get; set; }
        public int Count { get; set; }
    }
}
