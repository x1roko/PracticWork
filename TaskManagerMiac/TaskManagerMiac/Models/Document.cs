using System;
using System.Collections.Generic;

namespace TaskManagerMiac.Models;

public partial class Document
{
    public int IdDocument { get; set; }

    public string Title { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public string? Extension { get; set; }

    public int TaskBodyIdTask { get; set; }

    public bool IsDeleted { get; set; } = false;    

    public virtual TaskBody TaskBodyIdTaskNavigation { get; set; } = null!;
    public virtual FinancialTaskDocument FinancialTaskDocumentNavigation { get; set; } = null!;
}
