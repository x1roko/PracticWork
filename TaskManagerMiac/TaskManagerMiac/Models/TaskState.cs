using System;
using System.Collections.Generic;

namespace TaskManagerMiac.Models;

public partial class TaskState
{
    public int IdTaskState { get; set; }

    public string Title { get; set; } = null!;

    public virtual ICollection<TaskStep> TaskSteps { get; set; } = new List<TaskStep>();
}
