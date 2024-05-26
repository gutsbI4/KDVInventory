using System;
using System.Collections.Generic;

namespace api.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int RoleId { get; set; }

    public string? RefreshToken { get; set; }

    public DateTime? RefreshTokenExp { get; set; }

    public string? Image { get; set; }

    public bool IsArchive { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<Audit> Audit { get; set; } = new List<Audit>();

    public virtual Employee? Employee { get; set; }

    public virtual Role Role { get; set; } = null!;
}
