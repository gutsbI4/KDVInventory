using System;
using System.Collections.Generic;

namespace api.Models;

public partial class Audit
{
    public int Id { get; set; }

    public string Action { get; set; } = null!;

    public DateTime DateOfAction { get; set; }

    public int UserId { get; set; }

    public virtual User User { get; set; } = null!;
}
