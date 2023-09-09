namespace EddBotBackend.Shared.Models
{
    public class MkThreadsModel
    {
        public string Category { get; set; }
        public string Channel { get; set; }
        public List<string> Threads { get; set; }
    }
}
