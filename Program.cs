var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Добавляем аутентификацию и авторизацию
builder.Services.AddAuthentication("CookieAuth")
    .AddCookie("CookieAuth", options =>
    {
        options.LoginPath = "/Account/Login"; // Путь к странице входа
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Настраиваем маршрутизацию для MVC
app.UseStaticFiles(); // Поддержка статических файлов из wwwroot
app.UseRouting();

// Подключаем аутентификацию и авторизацию
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
