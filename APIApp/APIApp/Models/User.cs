using System;

namespace APIApp.Models
{
    public class User
    {
        public User()
        {
            // Empty constructor for EF Core
        }

        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        // public string Role { get; set; } = "User";
    }
}