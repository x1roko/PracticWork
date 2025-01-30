using System;
using System.Collections.Generic;

namespace TaskManagerMiac.Models;

public partial class TaskType
{
    public int IdTaskType { get; set; }

    public string Title { get; set; } = null!;

    public virtual ICollection<TaskBody> TaskBodies { get; set; } = new List<TaskBody>();

    public virtual ICollection<TaskTypePath> TaskTypePaths { get; set; } = new List<TaskTypePath>();
}
