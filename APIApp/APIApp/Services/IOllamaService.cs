using System.Threading.Tasks;

namespace APIApp.Services
{
    public interface IOllamaService
    {
        Task<string> GenerateReportAsync(string dataSummary, string userPrompt, string reportType);
    }
}