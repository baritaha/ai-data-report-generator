using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace APIApp.Services
{
    public interface IFileParserService
    {
        Task<string> GetDataSummary(IFormFile file);
    }
}