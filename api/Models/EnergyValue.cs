using System;
using System.Collections.Generic;

namespace api.Models;

public partial class EnergyValue
{
    public int ProductId { get; set; }

    public double? Proteins { get; set; }

    public double? Fats { get; set; }

    public double? Carbs { get; set; }

    public double? Calories { get; set; }

    public virtual Product Product { get; set; } = null!;
}
