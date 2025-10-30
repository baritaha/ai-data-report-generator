using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;

namespace APIApp.Services
{
    public class FileParserService : IFileParserService
    {
        private readonly ILogger<FileParserService> _logger;

        public FileParserService(ILogger<FileParserService> logger)
        {
            _logger = logger;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public async Task<string> GetDataSummary(IFormFile file)
        {
            try
            {
                var fileExtension = Path.GetExtension(file.FileName).ToLower();
                _logger.LogInformation($"Processing file: {file.FileName}, Type: {fileExtension}");

                return fileExtension switch
                {
                    ".csv" => await ParseCsvFile(file),
                    ".xlsx" or ".xls" => await ParseExcelFile(file),
                    _ => throw new ArgumentException($"Unsupported file format: {fileExtension}")
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing file");
                throw;
            }
        }

        private async Task<string> ParseCsvFile(IFormFile file)
        {
            using var stream = new StreamReader(file.OpenReadStream());
            var content = await stream.ReadToEndAsync();
            var lines = content.Split('\n')
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Select(line => line.Trim())
                .ToArray();

            if (lines.Length == 0)
                return "Empty CSV file";

            var headers = lines[0].Split(',').Length;
            var rowCount = lines.Length - 1;

            var summary = new StringBuilder();
            summary.AppendLine($"📊 CSV File Analysis");
            summary.AppendLine($"• File Name: {file.FileName}");
            summary.AppendLine($"• Total Rows: {rowCount}");
            summary.AppendLine($"• Columns: {headers}");
            summary.AppendLine("");
            summary.AppendLine("📋 Sample Data (First 3 rows):");
            summary.AppendLine("");

            for (int i = 0; i < Math.Min(3, lines.Length); i++)
            {
                summary.AppendLine($"{i + 1}: {lines[i]}");
            }

            return summary.ToString();
        }

        private async Task<string> ParseExcelFile(IFormFile file)
        {
            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            
            using var package = new ExcelPackage(stream);
            var worksheet = package.Workbook.Worksheets[0];
            
            var rowCount = worksheet.Dimension?.Rows ?? 0;
            var colCount = worksheet.Dimension?.Columns ?? 0;

            var summary = new StringBuilder();
            summary.AppendLine($"📊 Excel File Analysis");
            summary.AppendLine($"• File Name: {file.FileName}");
            summary.AppendLine($"• Total Rows: {rowCount - 1}");
            summary.AppendLine($"• Columns: {colCount}");
            summary.AppendLine($"• Worksheet: {worksheet.Name}");
            summary.AppendLine("");
            summary.AppendLine("📋 Sample Data:");
            summary.AppendLine("");

            // Headers
            if (rowCount > 0)
            {
                var headerRow = new List<string>();
                for (int col = 1; col <= colCount; col++)
                {
                    headerRow.Add(worksheet.Cells[1, col].Text?.ToString() ?? $"Column{col}");
                }
                summary.AppendLine($"Headers: {string.Join(" | ", headerRow)}");
                summary.AppendLine("");
            }

            // Sample data (first 2 data rows)
            for (int row = 2; row <= Math.Min(3, rowCount); row++)
            {
                var rowData = new List<string>();
                for (int col = 1; col <= colCount; col++)
                {
                    var cellValue = worksheet.Cells[row, col].Text?.ToString() ?? "";
                    rowData.Add(cellValue.Length > 20 ? cellValue.Substring(0, 20) + "..." : cellValue);
                }
                summary.AppendLine($"Row {row - 1}: {string.Join(" | ", rowData)}");
            }

            return summary.ToString();
        }
    }
}