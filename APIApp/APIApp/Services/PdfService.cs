using System;
using System.IO;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.Extensions.Logging;

namespace APIApp.Services
{
    public class PdfService : IPdfService
    {
        private readonly ILogger<PdfService> _logger;

        public PdfService(ILogger<PdfService> logger)
        {
            _logger = logger;
        }

        public byte[] GeneratePdf(string title, string content, string fileName, string reportType)
        {
            try
            {
                _logger.LogInformation("Starting PDF generation");
                
                using var memoryStream = new MemoryStream();
                var writer = new PdfWriter(memoryStream);
                var pdf = new PdfDocument(writer);
                var document = new Document(pdf);

                // Title
                document.Add(new Paragraph(title ?? "AI Generated Report")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(18)
                    .SetBold()
                    .SetMarginBottom(20));

                // Metadata
                document.Add(new Paragraph($"Generated from: {fileName ?? "Unknown file"}")
                    .SetFontSize(10)
                    .SetItalic());
                document.Add(new Paragraph($"Report Type: {reportType ?? "summary"}")
                    .SetFontSize(10)
                    .SetItalic());
                document.Add(new Paragraph($"Generated on: {DateTime.UtcNow:yyyy-MM-dd HH:mm} UTC")
                    .SetFontSize(10)
                    .SetItalic());

                document.Add(new Paragraph("\n"));

                // Content - handle special characters and formatting
                var safeContent = content ?? "No content generated";
                
                // Clean the content to remove any problematic characters
                safeContent = CleanContent(safeContent);
                
                var contentParagraph = new Paragraph(safeContent)
                    .SetFontSize(11)
                    .SetTextAlignment(TextAlignment.LEFT);
                document.Add(contentParagraph);

                document.Close();
                
                _logger.LogInformation("PDF generated successfully");
                return memoryStream.ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating PDF");
                
                // Fallback: Create a simple PDF with error message
                return CreateFallbackPdf(title ?? "Error Report", content ?? "No content", ex.Message);
            }
        }

        private string CleanContent(string content)
        {
            if (string.IsNullOrEmpty(content))
                return "No content available";

            // Remove or replace problematic characters for PDF generation
            return content
                .Replace("\u0000", "") // Remove null characters
                .Replace("\u0001", "") // Remove other control characters
                .Replace("\u0002", "")
                .Replace("\u0003", "")
                .Replace("\u0004", "")
                .Replace("\u0005", "")
                .Replace("\u0006", "")
                .Replace("\u0007", "")
                .Replace("\u0008", "")
                .Replace("\u000B", "")
                .Replace("\u000C", "")
                .Replace("\u000E", "")
                .Replace("\u000F", "");
        }

        private byte[] CreateFallbackPdf(string title, string content, string errorMessage)
        {
            try
            {
                using var memoryStream = new MemoryStream();
                var writer = new PdfWriter(memoryStream);
                var pdf = new PdfDocument(writer);
                var document = new Document(pdf);

                // Title
                document.Add(new Paragraph(title)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(18)
                    .SetBold()
                    .SetMarginBottom(20));

                // Error information
                document.Add(new Paragraph("Note: PDF generation encountered an issue")
                    .SetFontSize(10)
                    .SetItalic()
                    .SetFontColor(iText.Kernel.Colors.ColorConstants.RED));

                document.Add(new Paragraph($"Error: {errorMessage}")
                    .SetFontSize(8)
                    .SetItalic()
                    .SetFontColor(iText.Kernel.Colors.ColorConstants.RED));

                document.Add(new Paragraph("\n"));

                // Content
                var safeContent = content ?? "No content available";
                safeContent = CleanContent(safeContent);
                
                var contentParagraph = new Paragraph(safeContent)
                    .SetFontSize(11)
                    .SetTextAlignment(TextAlignment.LEFT);
                document.Add(contentParagraph);

                document.Close();
                return memoryStream.ToArray();
            }
            catch (Exception fallbackEx)
            {
                _logger.LogError(fallbackEx, "Even fallback PDF generation failed");
                
                // Last resort: return a very basic PDF
                return System.Text.Encoding.UTF8.GetBytes(
                    $"PDF Generation Failed\nTitle: {title}\nError: {fallbackEx.Message}\nContent: {content?.Substring(0, Math.Min(100, content.Length))}...");
            }
        }
    }
}