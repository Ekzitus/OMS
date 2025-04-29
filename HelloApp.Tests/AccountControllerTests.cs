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

namespace HelloApp.Tests;

public class AccountControllerTests
{
    [Fact]
    public void AccountController_Login_ReturnsViewResult()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        using var context = new ApplicationDbContext(options);
        var controller = new AccountController(context);

        // Act
        var result = controller.Login();

        // Assert
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task AccountController_Login_ValidCredentials_RedirectsToHomeIndex()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        using var context = new ApplicationDbContext(options);
        context.Users.Add(new User { Username = "admin", PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"), Role = "Admin" });
        context.SaveChanges();

        var controller = new AccountController(context);

        // Настраиваем поддельный HttpContext с сервисами
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

        httpContext.RequestServices = serviceProviderMock.Object;
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // Act
        var result = await controller.Login("admin", "password");

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        Assert.Equal("Home", redirectResult.ControllerName);
    }

    [Fact]
    public async Task AccountController_Login_InvalidCredentials_ReturnsViewWithError()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        using var context = new ApplicationDbContext(options);
        context.Users.Add(new User { Username = "admin", PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"), Role = "Admin" });
        context.SaveChanges();

        var controller = new AccountController(context);

        // Act
        var result = await controller.Login("wrongUser", "wrongPassword") as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Неверное имя пользователя или пароль", result?.ViewData["Error"] as string);
    }

    [Fact]
    public void AccountController_Logout_RedirectsToLogin()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        using var context = new ApplicationDbContext(options);
        var controller = new AccountController(context);

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

        // Act
        var result = controller.Logout();

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Login", redirectResult.ActionName);
    }
}