using System;
using System.ComponentModel.DataAnnotations;

namespace APIApp.Models
{
    public class DbReport
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        [Required]
        [MaxLength(500)]
        public string? Title { get; set; } = string.Empty;
        
        [Required]
        public string? Content { get; set; } = string.Empty;
        
        [Required]
        public string? Prompt { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(50)]
        public string? ReportType { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(255)]
        public string? FileName { get; set; } = string.Empty;
        
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
        
        public int? UserId { get; set; } // Optional: link to user who created the report
        public User? User { get; set; }
    }
}