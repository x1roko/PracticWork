using TaskManagerMiac.Models;

namespace TaskManagerMiac.Interfaces
{
    /// <summary>
    /// Интерфейс, который требуется реализовывать новым классам документов
    /// </summary>
    public interface IDocument
    {
        public int IdDocument { get; set; }
        public string DocumentName { get; }
        public Dictionary<string, string> GetParameters ();
        public abstract Document IdDocumentNavigation { get; set; }
    }
}
