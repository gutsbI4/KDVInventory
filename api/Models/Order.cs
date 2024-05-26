using System;
using System.Collections.Generic;

namespace api.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public int EmployeeId { get; set; }

    public DateTime DateOfOrder { get; set; }

    public DateTime? DateOfShipment { get; set; }

    public string? Commentary { get; set; }

    public string Address { get; set; } = null!;

    public bool IsShipment { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual ICollection<OrderProduct> OrderProduct { get; set; } = new List<OrderProduct>();
}
