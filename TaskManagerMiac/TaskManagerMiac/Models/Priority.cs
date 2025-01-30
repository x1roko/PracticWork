using System;
using System.Collections.Generic;

namespace TaskManagerMiac.Models;

public partial class Priority
{
    public int IdPriority { get; set; }

    public string Title { get; set; } = null!;

    public int Weight { get; set; }

    public virtual ICollection<TaskBody> TaskBodies { get; set; } = new List<TaskBody>();
}
