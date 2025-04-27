using HelloApp.Controllers;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace HelloApp.Tests;

public class HomeControllerTests
{
    [Fact]
    public void HomeController_Index_ReturnsViewResult()
    {
        // Arrange
        var controller = new HomeController();

        // Act
        var result = controller.Index();

        // Assert
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void HomeController_Index_ReturnsCorrectViewName()
    {
        // Arrange
        var controller = new HomeController();

        // Act
        var result = controller.Index() as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.True(result?.ViewName == null || result.ViewName == "Index"); // Проверяем, что имя представления либо null, либо "Index"
    }

    [Fact]
    public void HomeController_Index_HandlesException()
    {
        // Arrange
        var controller = new HomeController();

        // Здесь можно использовать Mock или другой подход для симуляции исключения, если метод вызывает внешние зависимости.

        // Act & Assert
        var exception = Record.Exception(() => controller.Index());
        Assert.Null(exception); // Проверяем, что исключение не выбрасывается
    }
}