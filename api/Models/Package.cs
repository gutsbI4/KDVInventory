using System;
using System.Collections.Generic;

namespace api.Models;

public partial class Package
{
    public int PackageId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<ProductDetails> ProductDetails { get; set; } = new List<ProductDetails>();
}
