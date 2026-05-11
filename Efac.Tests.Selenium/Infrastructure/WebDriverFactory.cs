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
        options.AddArgument("--disable-dev-shm-usage");
        options.AddArgument("--no-sandbox");
        options.AddArgument("--remote-debugging-port=0");
        options.AddArgument($"--user-data-dir={Path.Combine(Path.GetTempPath(), $"efac-selenium-{Guid.NewGuid():N}")}");

        return new ChromeDriver(options);
    }
}
