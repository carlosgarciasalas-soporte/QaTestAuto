using OpenQA.Selenium;

namespace Efac.Tests.Selenium.Pages;

public sealed class ClientesPage
{
    private readonly IWebDriver driver;

    public ClientesPage(IWebDriver driver)
    {
        this.driver = driver;
    }

    public IWebElement SearchInput => driver.FindElement(By.Id("input-search"));

    public IWebElement NewClientButton => driver.FindElement(By.Id("btn-new-client"));

    public IWebElement ClientModal => driver.FindElement(By.Id("cliente-modal"));

    public void Open(string baseUrl)
    {
        driver.Navigate().GoToUrl(baseUrl);
    }
}
