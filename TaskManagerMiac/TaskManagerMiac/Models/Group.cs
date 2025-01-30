using System;
using System.Collections.Generic;

namespace TaskManagerMiac.Models;

public partial class Group
{
    public int IdGroup { get; set; }

    public string Title { get; set; } = null!;

    public virtual ICollection<TaskBodyPath> TaskBodyPaths { get; set; } = new List<TaskBodyPath>();

    public virtual ICollection<TaskStep> TaskSteps { get; set; } = new List<TaskStep>();

    public virtual ICollection<TaskTypePath> TaskTypePaths { get; set; } = new List<TaskTypePath>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
