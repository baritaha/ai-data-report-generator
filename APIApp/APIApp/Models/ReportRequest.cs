namespace APIApp.Models
{
    public class ReportRequest
    {
        public IFormFile? File { get; set; } // This CANNOT be stored in database
        public string? Prompt { get; set; }
        public string? ReportType { get; set; }
    }
}