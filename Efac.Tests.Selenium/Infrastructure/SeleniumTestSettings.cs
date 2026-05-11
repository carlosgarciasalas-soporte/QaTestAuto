namespace Efac.Tests.Selenium.Infrastructure;

public static class SeleniumTestSettings
{
    public static string BaseUrl =>
        Environment.GetEnvironmentVariable("EFAC_BASE_URL") ?? "http://localhost:5100/";

    public static bool IsEnabled =>
        string.Equals(Environment.GetEnvironmentVariable("EFAC_RUN_SELENIUM"), "true", StringComparison.OrdinalIgnoreCase);

    public static bool IsHeadless =>
        !string.Equals(Environment.GetEnvironmentVariable("EFAC_SELENIUM_HEADLESS"), "false", StringComparison.OrdinalIgnoreCase);
}
