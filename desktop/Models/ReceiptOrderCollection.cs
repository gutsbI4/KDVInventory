using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace desktop.Models
{
    public class ReceiptOrderCollection
    {
        public IEnumerable<ReceiptOrder> ReceiptOrders { get; set; }
        public int Count { get; set; }
    }
}
