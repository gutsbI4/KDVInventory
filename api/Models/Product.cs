using System;
using System.Collections.Generic;

namespace api.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string? ProductNumber { get; set; }

    public long? Barcode { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal PriceOfSale { get; set; }

    public decimal? MinPriceOfSale { get; set; }

    public decimal? PurchasePrice { get; set; }

    public int? LowStockThreshold { get; set; }

    public int? CategoryId { get; set; }

    public int? PriceUnitId { get; set; }

    public bool IsDeleted { get; set; }

    public virtual Category? Category { get; set; }

    public virtual EnergyValue? EnergyValue { get; set; }

    public virtual ICollection<ExpenseOrderProduct> ExpenseOrderProduct { get; set; } = new List<ExpenseOrderProduct>();

    public virtual ICollection<OrderProduct> OrderProduct { get; set; } = new List<OrderProduct>();

    public virtual PriceUnit? PriceUnit { get; set; }

    public virtual ProductDetails? ProductDetails { get; set; }

    public virtual ICollection<ProductImage> ProductImage { get; set; } = new List<ProductImage>();

    public virtual ICollection<ReceiptOrderProduct> ReceiptOrderProduct { get; set; } = new List<ReceiptOrderProduct>();
}
