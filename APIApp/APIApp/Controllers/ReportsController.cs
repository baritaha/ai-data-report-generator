using Microsoft.AspNetCore.Mvc;
using APIApp.Models;
using APIApp.Services;

namespace APIApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly IOllamaService _ollamaService;
        private readonly IFileParserService _fileParserService;
        private readonly IPdfService _pdfService;
        private readonly ILogger<ReportsController> _logger;
        private static readonly List<GeneratedReport> _reports = new();

        public ReportsController(
            IOllamaService ollamaService,
            IFileParserService fileParserService,
            IPdfService pdfService,
            ILogger<ReportsController> logger)
        {
            _ollamaService = ollamaService;
            _fileParserService = fileParserService;
            _pdfService = pdfService;
            _logger = logger;
        }

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateReport([FromForm] ReportRequest request)
        {
            try
            {
                _logger.LogInformation("Starting report generation request");

                if (request.File == null || request.File.Length == 0)
                    return BadRequest(new { message = "No file uploaded" });

                if (string.IsNullOrWhiteSpace(request.Prompt))
                    return BadRequest(new { message = "Prompt is required" });

                // Parse file
                var dataSummary = await _fileParserService.GetDataSummary(request.File);
                _logger.LogInformation("File parsed successfully");

                // Generate AI report
                _logger.LogInformation("Sending to Ollama AI...");
                var aiReport = await _ollamaService.GenerateReportAsync(dataSummary, request.Prompt, request.ReportType);
                _logger.LogInformation("AI report generated successfully");

                // Create report object
                var report = new GeneratedReport
                {
                    Title = $"AI {request.ReportType} Report - {request.File.FileName}",
                    Content = aiReport,
                    Prompt = request.Prompt,
                    ReportType = request.ReportType,
                    FileName = request.File.FileName,
                    GeneratedAt = DateTime.UtcNow
                };

                _reports.Add(report);
                _logger.LogInformation($"Report generated with ID: {report.Id}");

                return Ok(report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating report");
                return StatusCode(500, new { message = $"Error generating report: {ex.Message}" });
            }
        }

        [HttpGet("{id}/download")]
        public IActionResult DownloadPdf(string id)
        {
            try
            {
                var report = _reports.FirstOrDefault(r => r.Id == id);
                if (report == null)
                    return NotFound(new { message = "Report not found" });

                _logger.LogInformation($"Generating PDF for report: {id}");
                var pdfBytes = _pdfService.GeneratePdf(
                    report.Title,
                    report.Content,
                    report.FileName,
                    report.ReportType
                );

                var fileName = $"{report.Title.Replace(":", "").Replace(" ", "_")}.pdf";
                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating PDF");
                return StatusCode(500, new { message = $"Error generating PDF: {ex.Message}" });
            }
        }

        [HttpGet("history")]
        public IActionResult GetReportHistory()
        {
            return Ok(_reports.OrderByDescending(r => r.GeneratedAt));
        }

        [HttpGet("{id}")]
        public IActionResult GetReport(string id)
        {
            var report = _reports.FirstOrDefault(r => r.Id == id);
            if (report == null)
                return NotFound(new { message = "Report not found" });

            return Ok(report);
        }
    }
}