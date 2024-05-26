using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace desktop.Models
{
    public class ExpenseOrderCollection
    {
        public IEnumerable<ExpenseOrder> ExpenseOrders { get; set; }
        public int Count { get; set; }
    }
}
