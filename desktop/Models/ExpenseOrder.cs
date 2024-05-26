using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace desktop.Models
{
    public class ExpenseOrder
    {
        public int Id { get; set; }

        public DateTime DateOfCreate { get; set; }

        public DateTime? DateOfExpense { get; set; }

        public bool IsExpense { get; set; }

        public string? Commentary { get; set; }

        public string Employee { get; set; }
        public decimal? Total { get; set; }
    }
}
