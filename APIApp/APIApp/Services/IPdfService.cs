namespace APIApp.Services
{
    public interface IPdfService
    {
        byte[] GeneratePdf(string title, string content, string fileName, string reportType);
    }
}