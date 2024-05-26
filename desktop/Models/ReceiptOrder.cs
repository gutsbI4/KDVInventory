using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace desktop.Models
{
    public class ReceiptOrder
    {
        public int Id { get; set; }

        public DateTime DateOfCreate { get; set; }

        public DateTime? DateOfReceipt { get; set; }

        public bool IsReceipt { get; set; }

        public string? Commentary { get; set; }

        public string Employee { get; set; }
        public decimal? Total { get; set; }
    }
}
