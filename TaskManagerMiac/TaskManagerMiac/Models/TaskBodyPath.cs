using System;
using System.Collections.Generic;

namespace TaskManagerMiac.Models;

public partial class TaskBodyPath
{
    public int TaskBodyIdTask { get; set; }

    public int GroupIdGroup { get; set; }

    public int Index { get; set; }

    public virtual Group GroupIdGroupNavigation { get; set; } = null!;

    public virtual TaskBody TaskBodyIdTaskNavigation { get; set; } = null!;
}
