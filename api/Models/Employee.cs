using System;
using System.Collections.Generic;

namespace api.Models;

public partial class Employee
{
    public int EmployeeId { get; set; }

    public string? Surname { get; set; }

    public string Name { get; set; } = null!;

    public string? MiddleName { get; set; }

    public string? PhoneNumber { get; set; }

    public virtual User EmployeeNavigation { get; set; } = null!;

    public virtual ICollection<ExpenseOrder> ExpenseOrder { get; set; } = new List<ExpenseOrder>();

    public virtual ICollection<Order> Order { get; set; } = new List<Order>();

    public virtual ICollection<ReceiptOrder> ReceiptOrder { get; set; } = new List<ReceiptOrder>();
}
