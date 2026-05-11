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

    [Fact]
    public void CreateNaturalClient_WhenSeleniumIsEnabled_AddsClientToTable()
    {
        if (!SeleniumTestSettings.IsEnabled)
        {
            return;
        }

        using var driver = WebDriverFactory.CreateChromeDriver();
        var page = new ClientesPage(driver);
        var nit = CreateUniqueNit();

        page.Open(SeleniumTestSettings.BaseUrl);
        page.CreateNaturalClient(nit, "Cliente", "Natural UI", "1990-01-01", $"natural.{nit}@qa.local");
        page.Search(nit);

        page.TableContains(nit).Should().BeTrue();
        page.TableContains("Cliente Natural UI").Should().BeTrue();
    }

    [Fact]
    public void CreateJuridicaClient_WhenSeleniumIsEnabled_AddsCompanyToTable()
    {
        if (!SeleniumTestSettings.IsEnabled)
        {
            return;
        }

        using var driver = WebDriverFactory.CreateChromeDriver();
        var page = new ClientesPage(driver);
        var nit = CreateUniqueNit();
        var razonSocial = $"Empresa QA {nit} S.A.S.";

        page.Open(SeleniumTestSettings.BaseUrl);
        page.CreateJuridicaClient(nit, razonSocial, $"juridica.{nit}@qa.local");
        page.Search(nit);

        page.TableContains(nit).Should().BeTrue();
        page.TableContains(razonSocial).Should().BeTrue();
    }

    [Fact]
    public void DuplicateNit_WhenSeleniumIsEnabled_ShowsValidationError()
    {
        if (!SeleniumTestSettings.IsEnabled)
        {
            return;
        }

        using var driver = WebDriverFactory.CreateChromeDriver();
        var page = new ClientesPage(driver);
        var nit = CreateUniqueNit();

        page.Open(SeleniumTestSettings.BaseUrl);
        page.CreateNaturalClient(nit, "Cliente", "Original", "1990-01-01", $"original.{nit}@qa.local");
        page.TryCreateDuplicateNaturalClient(nit, $"duplicado.{nit}@qa.local");

        page.WaitUntilAnyFormError().Should().Contain("Ya existe un cliente registrado con el NIT indicado");
    }

    [Fact]
    public void UnderageNaturalClient_WhenSeleniumIsEnabled_ShowsValidationError()
    {
        if (!SeleniumTestSettings.IsEnabled)
        {
            return;
        }

        using var driver = WebDriverFactory.CreateChromeDriver();
        var page = new ClientesPage(driver);
        var nit = CreateUniqueNit();

        page.Open(SeleniumTestSettings.BaseUrl);
        page.TryCreateUnderageNaturalClient(nit, $"menor.{nit}@qa.local");

        page.WaitUntilAnyFormError().Should().Contain("El cliente debe ser mayor de edad");
    }

    [Fact]
    public void EditClient_WhenSeleniumIsEnabled_UpdatesClientData()
    {
        if (!SeleniumTestSettings.IsEnabled)
        {
            return;
        }

        using var driver = WebDriverFactory.CreateChromeDriver();
        var page = new ClientesPage(driver);
        var nit = CreateUniqueNit();
        var updatedEmail = $"actualizado.{nit}@qa.local";

        page.Open(SeleniumTestSettings.BaseUrl);
        page.CreateNaturalClient(nit, "Cliente", "Editable", "1990-01-01", $"editable.{nit}@qa.local");
        page.EditEmailByNit(nit, updatedEmail);
        page.Search(nit);

        page.TableContains(nit).Should().BeTrue();
    }

    [Fact]
    public void DeleteClient_WhenSeleniumIsEnabled_RemovesClientFromTable()
    {
        if (!SeleniumTestSettings.IsEnabled)
        {
            return;
        }

        using var driver = WebDriverFactory.CreateChromeDriver();
        var page = new ClientesPage(driver);
        var nit = CreateUniqueNit();

        page.Open(SeleniumTestSettings.BaseUrl);
        page.CreateNaturalClient(nit, "Cliente", "Eliminar", "1990-01-01", $"eliminar.{nit}@qa.local");
        page.DeleteByNit(nit);
        page.SearchWithoutWaitingForMatch(nit);

        page.TableContains(nit).Should().BeFalse();
    }

    private static string CreateUniqueNit()
    {
        var randomPart = Random.Shared.Next(100000, 999999);
        return $"7{randomPart}{DateTimeOffset.UtcNow.Millisecond:D3}";
    }
}
