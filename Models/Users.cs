using System;
using System.Collections.Generic;

namespace ToDoAPI.Models;

public partial class Users
{
    public string IdUser { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Salt { get; set; }

    public virtual ICollection<Activities> Activities { get; set; } = new List<Activities>();
}
