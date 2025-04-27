using HelloApp.Controllers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using Moq;

namespace HelloApp.Tests;

public class AccountControllerTests
{
    [Fact]
    public void AccountController_Login_ReturnsViewResult()
    {
        // Arrange
        var controller = new AccountController();

        // Act
        var result = controller.Login();

        // Assert
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void AccountController_Login_ValidCredentials_RedirectsToHomeIndex()
    {
        // Arrange
        var controller = new AccountController();

        // Настраиваем поддельный HttpContext с сервисами
        var httpContext = new DefaultHttpContext();
        var authenticationServiceMock = new Mock<IAuthenticationService>();
        var serviceProviderMock = new Mock<IServiceProvider>();

        serviceProviderMock
            .Setup(sp => sp.GetService(typeof(IAuthenticationService)))
            .Returns(authenticationServiceMock.Object);

        httpContext.RequestServices = serviceProviderMock.Object;
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // Настраиваем поддельный IUrlHelper
        var urlHelperMock = new Mock<IUrlHelper>();
        controller.Url = urlHelperMock.Object;

        // Act
        var result = controller.Login("admin", "password");

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        Assert.Equal("Home", redirectResult.ControllerName);
    }

    [Fact]
    public void AccountController_Login_InvalidCredentials_ReturnsViewWithError()
    {
        // Arrange
        var controller = new AccountController();

        // Act
        var result = controller.Login("wrongUser", "wrongPassword") as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Неверное имя пользователя или пароль", result?.ViewData["Error"]);
    }

    [Fact]
    public void AccountController_Logout_RedirectsToLogin()
    {
        // Arrange
        var controller = new AccountController();

        // Настраиваем поддельный HttpContext с сервисами
        var httpContext = new DefaultHttpContext();
        var authenticationServiceMock = new Mock<IAuthenticationService>();
        var serviceProviderMock = new Mock<IServiceProvider>();

        serviceProviderMock
            .Setup(sp => sp.GetService(typeof(IAuthenticationService)))
            .Returns(authenticationServiceMock.Object);

        httpContext.RequestServices = serviceProviderMock.Object;
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // Настраиваем поддельный IUrlHelper
        var urlHelperMock = new Mock<IUrlHelper>();
        controller.Url = urlHelperMock.Object;

        // Act
        var result = controller.Logout();

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Login", redirectResult.ActionName);
    }
}