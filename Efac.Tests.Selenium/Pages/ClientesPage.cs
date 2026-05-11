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
        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(12));
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

    public IWebElement EmailInput => driver.FindElement(By.Id("input-email"));

    public IWebElement TelefonoInput => driver.FindElement(By.Id("input-telefono"));

    public IWebElement DireccionInput => driver.FindElement(By.Id("input-direccion"));

    public IWebElement CiudadInput => driver.FindElement(By.Id("input-ciudad"));

    public IWebElement ResponsabilidadSelect => driver.FindElement(By.Id("input-responsabilidad"));

    public IWebElement SaveClientButton => driver.FindElement(By.Id("btn-save-client"));

    public IWebElement FormValidationSummary => driver.FindElement(By.Id("form-validation-summary"));

    public IWebElement TableBody => driver.FindElement(By.Id("clientes-table-body"));

    public void Open(string baseUrl)
    {
        driver.Navigate().GoToUrl(baseUrl);
        WaitUntilLoaded();
    }

    public void OpenCreateModal()
    {
        ClickElement(NewClientButton);
        wait.Until(_ => ClientModal.GetDomAttribute("class")?.Contains("show", StringComparison.OrdinalIgnoreCase) == true);
    }

    public void SelectTipoPersona(string value)
    {
        var select = new SelectElement(TipoPersonaSelect);
        select.SelectByValue(value);
    }

    public void Search(string value)
    {
        SetInputValue(SearchInput, value);
        WaitUntilSearchApplied(value);
    }

    public void SearchWithoutWaitingForMatch(string value)
    {
        SetInputValue(SearchInput, value);
    }

    public bool TableContains(string value)
    {
        return TableBody.Text.Contains(value, StringComparison.OrdinalIgnoreCase);
    }

    public void TypeNitAndWaitForDv(string nit)
    {
        SetInputValue(NitInput, nit);
        wait.Until(_ => !string.IsNullOrWhiteSpace(DvInput.GetDomProperty("value")));
    }

    public void CreateNaturalClient(string nit, string nombres, string apellidos, string birthDate, string email)
    {
        OpenCreateModal();
        SelectTipoPersona("1");
        TypeNitAndWaitForDv(nit);
        SetInputValue(NombresInput, nombres);
        SetInputValue(ApellidosInput, apellidos);
        SetDateValue(FechaNacimientoInput, birthDate);
        FillCommonFields(email);
        SaveAndWaitForModalToClose();
        WaitUntilTableContains(nit);
    }

    public void CreateJuridicaClient(string nit, string razonSocial, string email)
    {
        OpenCreateModal();
        SelectTipoPersona("2");
        TypeNitAndWaitForDv(nit);
        SetInputValue(RazonSocialInput, razonSocial);
        FillCommonFields(email);
        SaveAndWaitForModalToClose();
        WaitUntilTableContains(nit);
    }

    public void TryCreateDuplicateNaturalClient(string nit, string email)
    {
        OpenCreateModal();
        SelectTipoPersona("1");
        TypeNitAndWaitForDv(nit);
        SetInputValue(NombresInput, "Duplicado");
        SetInputValue(ApellidosInput, "QA");
        SetDateValue(FechaNacimientoInput, "1990-01-01");
        FillCommonFields(email);
        ClickElement(SaveClientButton);
    }

    public void TryCreateUnderageNaturalClient(string nit, string email)
    {
        OpenCreateModal();
        SelectTipoPersona("1");
        TypeNitAndWaitForDv(nit);
        SetInputValue(NombresInput, "Cliente");
        SetInputValue(ApellidosInput, "Menor");
        SetDateValue(FechaNacimientoInput, "2010-01-01");
        FillCommonFields(email);
        ClickElement(SaveClientButton);
    }

    public void EditEmailByNit(string nit, string newEmail)
    {
        Search(nit);
        ClickElement(FindRowByText(nit).FindElement(By.CssSelector("button[aria-label='Editar']")));
        WaitUntilModalIsOpen();
        SetInputValue(EmailInput, newEmail);
        SaveAndWaitForModalToClose();
        WaitUntilTableContains(nit);
    }

    public void DeleteByNit(string nit)
    {
        Search(nit);
        ClickElement(FindRowByText(nit).FindElement(By.CssSelector("button[aria-label='Eliminar']")));
        wait.Until(_ => IsAlertOpen());
        driver.SwitchTo().Alert().Accept();
        wait.Until(_ => !TableContains(nit));
    }

    public void WaitUntilTableContains(string value)
    {
        wait.Until(_ => TableContains(value));
    }

    public string WaitUntilAnyFormError()
    {
        return wait.Until(_ =>
        {
            var className = FormValidationSummary.GetDomAttribute("class") ?? string.Empty;
            if (className.Contains("d-none", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            var text = FormValidationSummary.GetDomProperty("textContent") ?? FormValidationSummary.Text;
            return string.IsNullOrWhiteSpace(text) ? null : text;
        }) ?? string.Empty;
    }

    public void WaitUntilLoaded()
    {
        wait.Until(_ => SearchInput.Displayed);
        wait.Until(_ => NewClientButton.Displayed);
        wait.Until(_ => TableBody.FindElements(By.TagName("tr")).Count > 0);
    }

    private void FillCommonFields(string email)
    {
        SetInputValue(EmailInput, email);
        SetInputValue(TelefonoInput, "3009999999");
        SetInputValue(DireccionInput, "Calle QA 123");
        SetInputValue(CiudadInput, "11001");

        var select = new SelectElement(ResponsabilidadSelect);
        select.SelectByValue("2");
    }

    private void SaveAndWaitForModalToClose()
    {
        ClickElement(SaveClientButton);
        wait.Until(_ => ClientModal.GetDomAttribute("class")?.Contains("show", StringComparison.OrdinalIgnoreCase) == false);
    }

    private void WaitUntilModalIsOpen()
    {
        wait.Until(_ => ClientModal.GetDomAttribute("class")?.Contains("show", StringComparison.OrdinalIgnoreCase) == true);
    }

    private IWebElement FindRowByText(string value)
    {
        var row = wait.Until(_ => TableBody
            .FindElements(By.TagName("tr"))
            .FirstOrDefault(row => row.Text.Contains(value, StringComparison.OrdinalIgnoreCase)));

        return row ?? throw new NoSuchElementException($"No se encontro una fila con el texto '{value}'.");
    }

    private void WaitUntilSearchApplied(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return;
        }

        wait.Until(_ => TableBody.Text.Contains(value, StringComparison.OrdinalIgnoreCase));
    }

    private void SetInputValue(IWebElement input, string value)
    {
        ScrollIntoView(input);
        input.Clear();
        input.SendKeys(value);
    }

    private void SetDateValue(IWebElement input, string value)
    {
        ScrollIntoView(input);
        ((IJavaScriptExecutor)driver).ExecuteScript(
            "arguments[0].value = arguments[1]; arguments[0].dispatchEvent(new Event('input', { bubbles: true })); arguments[0].dispatchEvent(new Event('change', { bubbles: true }));",
            input,
            value);
    }

    private void ClickElement(IWebElement element)
    {
        ScrollIntoView(element);

        try
        {
            element.Click();
        }
        catch (ElementNotInteractableException)
        {
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", element);
        }
    }

    private void ScrollIntoView(IWebElement element)
    {
        ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView({ block: 'center', inline: 'nearest' });", element);
    }

    private bool IsAlertOpen()
    {
        try
        {
            driver.SwitchTo().Alert();
            return true;
        }
        catch (NoAlertPresentException)
        {
            return false;
        }
    }
}
