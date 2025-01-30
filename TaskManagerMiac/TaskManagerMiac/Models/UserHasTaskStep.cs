using System;
using System.Collections.Generic;

namespace TaskManagerMiac.Models;

public partial class UserHasTaskStep
{
    public int IdUser { get; set; }

    public int IdTaskStep { get; set; }

    public bool IsResponsible { get; set; }
    public bool IsChecked { get; set; }

    public virtual TaskStep IdTaskStepNavigation { get; set; } = null!;

    public virtual User IdUserNavigation { get; set; } = null!;
}
