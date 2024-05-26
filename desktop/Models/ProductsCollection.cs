using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace desktop.Models
{
    public class ProductsCollection
    {
        public IEnumerable<Product> Products { get; set; }
        public int Count { get; set; }
    }
}
