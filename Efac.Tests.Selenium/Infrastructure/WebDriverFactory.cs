using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Efac.Tests.Selenium.Infrastructure;

public static class WebDriverFactory
{
    public static IWebDriver CreateChromeDriver()
    {
        var options = new ChromeOptions();
        options.AddArgument("--headless=new");
        options.AddArgument("--window-size=1366,768");
        options.AddArgument("--disable-gpu");

        return new ChromeDriver(options);
    }
}
