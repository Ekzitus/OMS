using HelloApp.Data;
using HelloApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelloApp.Controllers;

public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminController(ApplicationDbContext context)
    {
        _context = context;
    }

    [Authorize(Roles = "Admin")] // Доступ только для администраторов
    [HttpGet]
    public IActionResult Index()
    {
        try
        {
            // Получаем список пользователей из базы данных
            var users = _context.Users?.ToList() ?? new List<User>();
            return View(users);
        }
        catch (Exception ex)
        {
            // Логируем ошибку (можно использовать ILogger)
            Console.WriteLine(ex.Message);
            return View("Error"); // Возвращаем страницу ошибки
        }
    }
}