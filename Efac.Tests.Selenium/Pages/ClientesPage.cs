using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Efac.Tests.Selenium.Pages;

public sealed class ClientesPage
{
    private readonly IWebDriver driver;
    private readonly WebDriverWait wait;

    public ClientesPage(IWebDriver driver)
    {
        this.driver = driver;
        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(8));
    }

    public IWebElement SearchInput => driver.FindElement(By.Id("input-search"));

    public IWebElement NewClientButton => driver.FindElement(By.Id("btn-new-client"));

    public IWebElement ClientModal => driver.FindElement(By.Id("cliente-modal"));

    public IWebElement TipoPersonaSelect => driver.FindElement(By.Id("input-tipo-persona"));

    public IWebElement NitInput => driver.FindElement(By.Id("input-nit"));

    public IWebElement DvInput => driver.FindElement(By.Id("input-dv"));

    public IWebElement NombresInput => driver.FindElement(By.Id("input-nombres"));

    public IWebElement ApellidosInput => driver.FindElement(By.Id("input-apellidos"));

    public IWebElement FechaNacimientoInput => driver.FindElement(By.Id("input-fecha-nacimiento"));

    public IWebElement RazonSocialInput => driver.FindElement(By.Id("input-razon-social"));

    public IWebElement TableBody => driver.FindElement(By.Id("clientes-table-body"));

    public void Open(string baseUrl)
    {
        driver.Navigate().GoToUrl(baseUrl);
        WaitUntilLoaded();
    }

    public void OpenCreateModal()
    {
        NewClientButton.Click();
        wait.Until(_ => ClientModal.GetDomAttribute("class")?.Contains("show", StringComparison.OrdinalIgnoreCase) == true);
    }

    public void SelectTipoPersona(string value)
    {
        var select = new SelectElement(TipoPersonaSelect);
        select.SelectByValue(value);
    }

    public void Search(string value)
    {
        SearchInput.Clear();
        SearchInput.SendKeys(value);
    }

    public bool TableContains(string value)
    {
        return TableBody.Text.Contains(value, StringComparison.OrdinalIgnoreCase);
    }

    public void TypeNitAndWaitForDv(string nit)
    {
        NitInput.Clear();
        NitInput.SendKeys(nit);
        wait.Until(_ => !string.IsNullOrWhiteSpace(DvInput.GetDomProperty("value")));
    }

    public void WaitUntilLoaded()
    {
        wait.Until(_ => SearchInput.Displayed);
        wait.Until(_ => NewClientButton.Displayed);
        wait.Until(_ => TableBody.FindElements(By.TagName("tr")).Count > 0);
    }
}
