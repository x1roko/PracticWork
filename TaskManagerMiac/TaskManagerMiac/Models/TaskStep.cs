using System;
using System.Collections.Generic;

namespace TaskManagerMiac.Models;

public partial class TaskStep
{
    public int IdTaskStep { get; set; }

    public string Description { get; set; } = null!;

    public int IdTaskState { get; set; }

    public int IdTask { get; set; }

    public int GroupIdPerformer { get; set; }

    public DateTime? DeadlineDate { get; set; }

    public DateTime CreationDate { get; set; }

    public DateTime? ChangeStateDate { get; set; }

    public DateTime? PerformerDate { get; set; }

    public int Index { get; set; }

    public virtual Group GroupIdPerformerNavigation { get; set; } = null!;

    public virtual TaskBody IdTaskNavigation { get; set; } = null!;

    public virtual TaskState IdTaskStateNavigation { get; set; } = null!;

    public virtual ICollection<UserHasTaskStep> UserHasTaskSteps { get; set; } = new List<UserHasTaskStep>();
}
