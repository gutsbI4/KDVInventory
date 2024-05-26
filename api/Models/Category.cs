using System;
using System.Collections.Generic;

namespace api.Models;

public partial class Category
{
    public int CategoryId { get; set; }

    public string Name { get; set; } = null!;

    public string? Icon { get; set; }

    public virtual ICollection<Product> Product { get; set; } = new List<Product>();
}
