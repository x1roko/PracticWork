using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagerMiac.Models;

public partial class TaskBody
{
    public int IdTask { get; set; }

    [Required]
    public string Title { get; set; } = null!;
    [Required]
    public string Description { get; set; } = null!;

    public int IdUserCreator { get; set; }
    [Required]
    public int IdPriority { get; set; }
    [Required]
    public int IdTaskType { get; set; }

    public DateTime CreationDate { get; set; }

    public DateTime? DeadlineDate { get; set; }

    public virtual ICollection<Document> Documents { get; set; } = new List<Document>();

    public virtual Priority IdPriorityNavigation { get; set; } = null!;

    public virtual TaskType IdTaskTypeNavigation { get; set; } = null!;

    public virtual User IdUserCreatorNavigation { get; set; } = null!;

    public virtual ICollection<TaskBodyPath> TaskBodyPaths { get; set; } = new List<TaskBodyPath>();

    public virtual ICollection<TaskStep> TaskSteps { get; set; } = new List<TaskStep>();
    public virtual ICollection<TaskBodyComment> Commentaries { get; set; } = new List<TaskBodyComment>();

    [NotMapped]
    public string State
    {
        get
        {
            if (TaskSteps.All(ts => ts.IdTaskState == 3))
            {
                return "Одобрена";
            }
            else if (TaskSteps.Any(ts => ts.IdTaskState == 4))
            {
                return "Отклонена";
            }
            else if (TaskSteps.Any(ts => ts.IdTaskState != 1))
            {
                return "В работе";
            }
            else
            {
                return "Создана";
            }
        }
    }
}
