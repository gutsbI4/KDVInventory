using System;
using System.Collections.Generic;

namespace api.Models;

public partial class ExpenseOrder
{
    public int Id { get; set; }

    public DateTime DateOfCreate { get; set; }

    public DateTime? DateOfExpense { get; set; }

    public bool IsExpense { get; set; }

    public string? Commentary { get; set; }

    public int EmployeeId { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual ICollection<ExpenseOrderProduct> ExpenseOrderProduct { get; set; } = new List<ExpenseOrderProduct>();
}
