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

    [Fact]
    public void Search_WhenSeleniumIsEnabled_FiltersClientesByNit()
    {
        if (!SeleniumTestSettings.IsEnabled)
        {
            return;
        }

        using var driver = WebDriverFactory.CreateChromeDriver();
        var page = new ClientesPage(driver);

        page.Open(SeleniumTestSettings.BaseUrl);
        page.Search("900373913");

        page.TableContains("900373913").Should().BeTrue();
        page.TableContains("1020304050").Should().BeFalse();
    }

    [Fact]
    public void TipoPersona_WhenSeleniumIsEnabled_TogglesNaturalAndJuridicaFields()
    {
        if (!SeleniumTestSettings.IsEnabled)
        {
            return;
        }

        using var driver = WebDriverFactory.CreateChromeDriver();
        var page = new ClientesPage(driver);

        page.Open(SeleniumTestSettings.BaseUrl);
        page.OpenCreateModal();

        page.NombresInput.Enabled.Should().BeTrue();
        page.ApellidosInput.Enabled.Should().BeTrue();
        page.FechaNacimientoInput.Enabled.Should().BeTrue();
        page.RazonSocialInput.Enabled.Should().BeFalse();

        page.SelectTipoPersona("2");

        page.NombresInput.Enabled.Should().BeFalse();
        page.ApellidosInput.Enabled.Should().BeFalse();
        page.FechaNacimientoInput.Enabled.Should().BeFalse();
        page.RazonSocialInput.Enabled.Should().BeTrue();
    }

    [Fact]
    public void NitInput_WhenSeleniumIsEnabled_CalculatesDvFromApi()
    {
        if (!SeleniumTestSettings.IsEnabled)
        {
            return;
        }

        using var driver = WebDriverFactory.CreateChromeDriver();
        var page = new ClientesPage(driver);

        page.Open(SeleniumTestSettings.BaseUrl);
        page.OpenCreateModal();
        page.TypeNitAndWaitForDv("8001972684");

        page.NitInput.GetDomProperty("value").Should().Be("8001972684");
        page.DvInput.GetDomProperty("value").Should().Be("9");
    }
}
