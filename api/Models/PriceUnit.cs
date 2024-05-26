using System;
using System.Collections.Generic;

namespace api.Models;

public partial class PriceUnit
{
    public int Id { get; set; }

    public string Unit { get; set; } = null!;

    public virtual ICollection<Product> Product { get; set; } = new List<Product>();
}
