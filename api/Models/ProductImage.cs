using System;
using System.Collections.Generic;

namespace api.Models;

public partial class ProductImage
{
    public int ImageId { get; set; }

    public int ProductId { get; set; }

    public string Path { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
