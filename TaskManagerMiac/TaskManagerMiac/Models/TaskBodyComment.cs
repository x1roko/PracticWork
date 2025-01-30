namespace TaskManagerMiac.Models
{
    public class TaskBodyComment
    {
        public int IdComment { get; set; }
        public string Text { get; set; }

        public int IdTask { get; set; }
        public DateTime PostDate { get; set; }
        public virtual TaskBody IdTaskNavigation { get; set; } = null!;
        public int IdUser{ get; set; }
        public virtual User IdUserNavigation { get; set; } = null!;
    }
}
