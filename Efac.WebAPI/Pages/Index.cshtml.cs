using Efac.Domain.Enums;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Efac.WebAPI.Pages;

public sealed class IndexModel : PageModel
{
    public ClienteFormModel Input { get; } = new();
}

public sealed class ClienteFormModel
{
    public TipoPersona TipoPersona { get; set; } = TipoPersona.Natural;
    public string Nit { get; set; } = string.Empty;
    public string? Nombres { get; set; }
    public string? Apellidos { get; set; }
    public string? RazonSocial { get; set; }
    public DateOnly? FechaNacimiento { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public string CiudadCodigoMunicipio { get; set; } = string.Empty;
    public ResponsabilidadFiscal ResponsabilidadFiscal { get; set; } = ResponsabilidadFiscal.ResponsableIva;
}
