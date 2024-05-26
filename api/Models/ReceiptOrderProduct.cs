using System;
using System.Collections.Generic;

namespace api.Models;

public partial class ReceiptOrderProduct
{
    public int ProductId { get; set; }

    public int ReceiptOrderId { get; set; }

    public int Quantity { get; set; }

    public decimal PurchasePrice { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual ReceiptOrder ReceiptOrder { get; set; } = null!;
}
