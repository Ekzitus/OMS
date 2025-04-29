using HelloApp.Controllers;
using HelloApp.Data;
using HelloApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace HelloApp.Tests;

public class AccountControllerTests
{
    [Fact]
    public void AccountController_Login_ReturnsViewResult()
    {
        // Тест проверяет, что метод Login возвращает представление (ViewResult).
        // Arrange: создаем контекст базы данных и контроллер.
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase") // Используем базу данных в памяти для тестов.
            .Options;
        using var context = new ApplicationDbContext(options);
        var controller = new AccountController(context);

        // Act: вызываем метод Login.
        var result = controller.Login();

        // Assert: проверяем, что результат - это ViewResult.
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task AccountController_Login_ValidCredentials_RedirectsToHomeIndex()
    {
        // Тест проверяет, что при вводе правильных учетных данных пользователь перенаправляется на Home/Index.
        // Arrange: создаем контекст базы данных, добавляем тестового пользователя и создаем контроллер.
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        using var context = new ApplicationDbContext(options);
        context.Users.Add(new User { Username = "admin", PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"), Role = "Admin" });
        context.SaveChanges();

        var controller = new AccountController(context);

        // Настраиваем поддельный HttpContext с необходимыми сервисами.
        var httpContext = new DefaultHttpContext();
        var authenticationServiceMock = new Mock<IAuthenticationService>(); // Мокаем сервис аутентификации.
        var serviceProviderMock = new Mock<IServiceProvider>();
        var urlHelperFactoryMock = new Mock<IUrlHelperFactory>();
        var urlHelperMock = new Mock<IUrlHelper>();

        urlHelperFactoryMock
            .Setup(factory => factory.GetUrlHelper(It.IsAny<ActionContext>()))
            .Returns(urlHelperMock.Object);

        serviceProviderMock
            .Setup(sp => sp.GetService(typeof(IAuthenticationService)))
            .Returns(authenticationServiceMock.Object);
        serviceProviderMock
            .Setup(sp => sp.GetService(typeof(IUrlHelperFactory)))
            .Returns(urlHelperFactoryMock.Object);

        httpContext.RequestServices = serviceProviderMock.Object;
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // Act: вызываем метод Login с правильными учетными данными.
        var result = await controller.Login("admin", "password");

        // Assert: проверяем, что результат - это перенаправление на Home/Index.
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        Assert.Equal("Home", redirectResult.ControllerName);
    }

    [Fact]
    public async Task AccountController_Login_InvalidCredentials_ReturnsViewWithError()
    {
        // Тест проверяет, что при вводе неправильных учетных данных возвращается представление с сообщением об ошибке.
        // Arrange: создаем контекст базы данных, добавляем тестового пользователя и создаем контроллер.
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        using var context = new ApplicationDbContext(options);
        context.Users.Add(new User { Username = "admin", PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"), Role = "Admin" });
        context.SaveChanges();

        var controller = new AccountController(context);

        // Act: вызываем метод Login с неправильными учетными данными.
        var result = await controller.Login("wrongUser", "wrongPassword") as ViewResult;

        // Assert: проверяем, что результат содержит сообщение об ошибке.
        Assert.NotNull(result);
        Assert.Equal("Неверное имя пользователя или пароль", result?.ViewData["Error"] as string);
    }

    [Fact]
    public void AccountController_Logout_RedirectsToLogin()
    {
        // Тест проверяет, что метод Logout перенаправляет пользователя на страницу входа (Login).
        // Arrange: создаем контекст базы данных и контроллер.
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        using var context = new ApplicationDbContext(options);
        var controller = new AccountController(context);

        // Настраиваем поддельный HttpContext с необходимыми сервисами.
        var httpContext = new DefaultHttpContext();
        var authenticationServiceMock = new Mock<IAuthenticationService>();
        var serviceProviderMock = new Mock<IServiceProvider>();
        var urlHelperFactoryMock = new Mock<IUrlHelperFactory>();

        serviceProviderMock
            .Setup(sp => sp.GetService(typeof(IAuthenticationService)))
            .Returns(authenticationServiceMock.Object);
        serviceProviderMock
            .Setup(sp => sp.GetService(typeof(IUrlHelperFactory)))
            .Returns(urlHelperFactoryMock.Object);

        httpContext.RequestServices = serviceProviderMock.Object;
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // Act: вызываем метод Logout.
        var result = controller.Logout();

        // Assert: проверяем, что результат - это перенаправление на Login.
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Login", redirectResult.ActionName);
    }

    [Fact]
    public void AccountController_Register_ReturnsViewResult()
    {
        // Тест проверяет, что метод Register возвращает представление (ViewResult).
        // Arrange: создаем контекст базы данных и контроллер.
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        using var context = new ApplicationDbContext(options);
        var controller = new AccountController(context);

        // Act: вызываем метод Register.
        var result = controller.Register();

        // Assert: проверяем, что результат - это ViewResult.
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task AccountController_Register_ValidCredentials_RedirectsToHomeIndex()
    {
        // Тест проверяет, что при успешной регистрации пользователь перенаправляется на Home/Index.
        // Arrange: создаем контекст базы данных и контроллер.
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        using var context = new ApplicationDbContext(options);
        var controller = new AccountController(context);

        // Настраиваем поддельный HttpContext с необходимыми сервисами.
        var httpContext = new DefaultHttpContext();
        var authenticationServiceMock = new Mock<IAuthenticationService>();
        var serviceProviderMock = new Mock<IServiceProvider>();
        var urlHelperFactoryMock = new Mock<IUrlHelperFactory>();
        var urlHelperMock = new Mock<IUrlHelper>();

        urlHelperFactoryMock
            .Setup(factory => factory.GetUrlHelper(It.IsAny<ActionContext>()))
            .Returns(urlHelperMock.Object);

        serviceProviderMock
            .Setup(sp => sp.GetService(typeof(IAuthenticationService)))
            .Returns(authenticationServiceMock.Object);
        serviceProviderMock
            .Setup(sp => sp.GetService(typeof(IUrlHelperFactory)))
            .Returns(urlHelperFactoryMock.Object);

        var tempDataDictionaryFactoryMock = new Mock<ITempDataDictionaryFactory>();
        serviceProviderMock
            .Setup(sp => sp.GetService(typeof(ITempDataDictionaryFactory)))
            .Returns(tempDataDictionaryFactoryMock.Object);

        httpContext.RequestServices = serviceProviderMock.Object;
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // Act: вызываем метод Register с правильными учетными данными.
        var result = await controller.Register("admin", "password");

        // Assert: проверяем, что результат - это ViewResult.
        var redirectResult = Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task AccountController_Register_InvalidCredentials_ReturnsViewWithError()
    {
        // Тест проверяет, что при вводе неправильных учетных данных возвращается представление с сообщением об ошибке.
        // Arrange: создаем контекст базы данных, добавляем тестового пользователя и создаем контроллер.
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        using var context = new ApplicationDbContext(options);

        var controller = new AccountController(context);

        // Act: вызываем метод Login с неправильными учетными данными.
        var result = await controller.Register("", "") as ViewResult;

        // Assert: проверяем, что результат содержит сообщение об ошибке.
        Assert.NotNull(result);
        Assert.Equal("Имя пользователя и пароль обязательны.", result?.ViewData["Error"] as string);
    }
}