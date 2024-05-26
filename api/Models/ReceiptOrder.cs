using System;
using System.Collections.Generic;

namespace api.Models;

public partial class ReceiptOrder
{
    public int Id { get; set; }

    public DateTime DateOfCreate { get; set; }

    public DateTime? DateOfReceipt { get; set; }

    public bool IsReceipt { get; set; }

    public string? Commentary { get; set; }

    public int EmployeeId { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual ICollection<ReceiptOrderProduct> ReceiptOrderProduct { get; set; } = new List<ReceiptOrderProduct>();
}
