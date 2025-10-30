namespace APIApp.Models
{
    public class GeneratedReport
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? Prompt { get; set; }
        public string? ReportType { get; set; }
        public string? FileName { get; set; }
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    }
}