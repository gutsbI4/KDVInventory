using System;
using System.Collections.Generic;

namespace api.Models;

public partial class Role
{
    public int RoleId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<User> User { get; set; } = new List<User>();
}
