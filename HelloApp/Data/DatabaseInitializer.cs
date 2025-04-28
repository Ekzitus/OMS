using HelloApp.Models;
using Microsoft.EntityFrameworkCore;

namespace HelloApp.Data
{
    public static class DatabaseInitializer
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (!context.Users.Any())
            {
                context.Users.Add(new User
                {
                    Username = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    Role = "Admin"
                });

                context.Users.Add(new User
                {
                    Username = "user",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("user123"),
                    Role = "User"
                });

                context.SaveChanges();
            }
        }
    }
}