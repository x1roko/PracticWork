using System;
using System.Collections.Generic;

namespace TaskManagerMiac.Models;

public partial class TaskTypePath
{
    public int IdTaskType { get; set; }

    public int IdGroup { get; set; }

    public int Index { get; set; }

    public virtual Group IdGroupNavigation { get; set; } = null!;

    public virtual TaskType IdTaskTypeNavigation { get; set; } = null!;
}
