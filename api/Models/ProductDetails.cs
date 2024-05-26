using System;
using System.Collections.Generic;

namespace api.Models;

public partial class ProductDetails
{
    public int ProductId { get; set; }

    public int? BrandId { get; set; }

    public int? ManufacturerId { get; set; }

    public string? CountryOfProduction { get; set; }

    public int? ShelfLife { get; set; }

    public double? Weight { get; set; }

    public int? PackageId { get; set; }

    public int? TypeId { get; set; }

    public int? TasteId { get; set; }

    public int? FillingId { get; set; }

    public int? DietId { get; set; }

    public virtual Brand? Brand { get; set; }

    public virtual Diet? Diet { get; set; }

    public virtual Filling? Filling { get; set; }

    public virtual Manufacturer? Manufacturer { get; set; }

    public virtual Package? Package { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual Taste? Taste { get; set; }

    public virtual Type? Type { get; set; }
}
