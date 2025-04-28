namespace HelloApp.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; } // Хэш пароля
        public string Role { get; set; } // Роль пользователя
    }
}