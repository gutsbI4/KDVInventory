using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace desktop.Models
{
    public class Order
    {
        public int OrderId { get; set; }

        public string Employee { get; set; }

        public DateTime DateOfOrder { get; set; }

        public DateTime? DateOfShipment { get; set; }

        public string? Commentary { get; set; }

        public string Address { get; set; } = null!;

        public bool IsShipment { get; set; }
        public decimal Total { get; set; }
    }
}
