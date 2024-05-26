using System;
using System.Collections.Generic;

namespace api.Models;

public partial class ExpenseOrderProduct
{
    public int ProductId { get; set; }

    public int ExpenseOrderId { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public virtual ExpenseOrder ExpenseOrder { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
