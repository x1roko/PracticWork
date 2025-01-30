using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagerMiac.Models;

public partial class User
{
    public int IdUser { get; set; }

    // login
    public string Snils { get; set; } = null!;

    public int IdGroup { get; set; }

    public string Firstname { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Patronymic { get; set; }

    public int IdRole { get; set; }

    public DateTime RegistrationDate { get; set; }

    public bool IsActive { get; set; }

    public DateTime LastEnterDate { get; set; }

    [NotMapped]
    public string FullName
    {
        get
        {
            string fullName = $"{Surname} {Firstname[0]}.";
            if (Patronymic != null)
            {
                fullName += $" {Patronymic[0]}.";
            }
            return fullName;
        }
    }

    public virtual Group IdGroupNavigation { get; set; } = null!;

    public virtual Role IdRoleNavigation { get; set; } = null!;

    public virtual ICollection<TaskBody> TaskBodies { get; set; } = new List<TaskBody>();

    public virtual ICollection<UserHasTaskStep> UserHasTaskSteps { get; set; } = new List<UserHasTaskStep>();
    public virtual ICollection<TaskBodyComment> Commentaries { get; set; } = new List<TaskBodyComment>();
}
