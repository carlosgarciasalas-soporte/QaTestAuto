using Efac.Tests.Selenium.Infrastructure;
using Efac.Tests.Selenium.Pages;
using FluentAssertions;

namespace Efac.Tests.Selenium.Tests;

public sealed class ClientesUiSmokeTests
{
    [Fact]
    public void HomePage_WhenSeleniumIsEnabled_LoadsMainQaControls()
    {
        if (!SeleniumTestSettings.IsEnabled)
        {
            return;
        }

        using var driver = WebDriverFactory.CreateChromeDriver();
        var page = new ClientesPage(driver);

        page.Open(SeleniumTestSettings.BaseUrl);

        driver.Title.Should().Contain("Clientes");
        page.SearchInput.Displayed.Should().BeTrue();
        page.NewClientButton.Displayed.Should().BeTrue();
    }
}
