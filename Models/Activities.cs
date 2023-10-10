using System;
using System.Collections.Generic;

namespace ToDoAPI.Models;

public partial class Activities
{
    public uint IdActivity { get; set; }

    public string Name { get; set; } = null!;

    public DateTime When { get; set; }

    public string IdUser { get; set; } = null!;

    public virtual Users IdUserNavigation { get; set; } = null!;
}
