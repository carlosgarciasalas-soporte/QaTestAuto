using Efac.Domain.Enums;
using Efac.Domain.Exceptions;
using Efac.Domain.Services;

namespace Efac.Domain.Entities;

public sealed class Cliente
{
    private Cliente()
    {
    }

    public Cliente(
        Guid id,
        TipoPersona tipoPersona,
        string nit,
        string? nombres,
        string? apellidos,
        string? razonSocial,
        DateOnly? fechaNacimiento,
        string email,
        string telefono,
        string direccion,
        string ciudadCodigoMunicipio,
        ResponsabilidadFiscal responsabilidadFiscal)
    {
        Id = id == Guid.Empty ? Guid.NewGuid() : id;
        Update(
            tipoPersona,
            nit,
            nombres,
            apellidos,
            razonSocial,
            fechaNacimiento,
            email,
            telefono,
            direccion,
            ciudadCodigoMunicipio,
            responsabilidadFiscal);
    }

    public Guid Id { get; private set; }
    public TipoPersona TipoPersona { get; private set; }
    public string Nit { get; private set; } = string.Empty;
    public int Dv { get; private set; }
    public string? Nombres { get; private set; }
    public string? Apellidos { get; private set; }
    public string? RazonSocial { get; private set; }
    public DateOnly? FechaNacimiento { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string Telefono { get; private set; } = string.Empty;
    public string Direccion { get; private set; } = string.Empty;
    public string CiudadCodigoMunicipio { get; private set; } = string.Empty;
    public ResponsabilidadFiscal ResponsabilidadFiscal { get; private set; }

    public string NombreCompleto =>
        TipoPersona == TipoPersona.Juridica
            ? RazonSocial ?? string.Empty
            : string.Join(' ', new[] { Nombres, Apellidos }.Where(value => !string.IsNullOrWhiteSpace(value)));

    public void Update(
        TipoPersona tipoPersona,
        string nit,
        string? nombres,
        string? apellidos,
        string? razonSocial,
        DateOnly? fechaNacimiento,
        string email,
        string telefono,
        string direccion,
        string ciudadCodigoMunicipio,
        ResponsabilidadFiscal responsabilidadFiscal)
    {
        var normalizedNit = DianModulo11Calculator.NormalizeNit(nit);

        EnsureRequired(normalizedNit, "El NIT es obligatorio");
        EnsureRequired(email, "El email es obligatorio");
        EnsureRequired(telefono, "El telefono es obligatorio");
        EnsureRequired(direccion, "La direccion es obligatoria");
        EnsureRequired(ciudadCodigoMunicipio, "La ciudad es obligatoria");

        if (tipoPersona == TipoPersona.Natural)
        {
            EnsureRequired(nombres, "Los nombres son obligatorios para persona natural");
            EnsureRequired(apellidos, "Los apellidos son obligatorios para persona natural");

            if (fechaNacimiento is null)
            {
                throw new DomainValidationException("La fecha de nacimiento es obligatoria para persona natural");
            }

            if (!IsAdult(fechaNacimiento.Value, DateOnly.FromDateTime(DateTime.UtcNow)))
            {
                throw new DomainValidationException("El cliente debe ser mayor de edad");
            }

            razonSocial = null;
        }
        else
        {
            EnsureRequired(razonSocial, "La razon social es obligatoria para persona juridica");
            nombres = null;
            apellidos = null;
            fechaNacimiento = null;
        }

        TipoPersona = tipoPersona;
        Nit = normalizedNit;
        Dv = DianModulo11Calculator.CalculateVerificationDigit(normalizedNit);
        Nombres = NormalizeOptionalText(nombres);
        Apellidos = NormalizeOptionalText(apellidos);
        RazonSocial = NormalizeOptionalText(razonSocial);
        FechaNacimiento = fechaNacimiento;
        Email = NormalizeRequiredText(email);
        Telefono = NormalizeRequiredText(telefono);
        Direccion = NormalizeRequiredText(direccion);
        CiudadCodigoMunicipio = NormalizeRequiredText(ciudadCodigoMunicipio);
        ResponsabilidadFiscal = responsabilidadFiscal;
    }

    private static bool IsAdult(DateOnly birthDate, DateOnly today)
    {
        var age = today.Year - birthDate.Year;
        if (birthDate > today.AddYears(-age))
        {
            age--;
        }

        return age >= 18;
    }

    private static void EnsureRequired(string? value, string message)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainValidationException(message);
        }
    }

    private static string NormalizeRequiredText(string value)
    {
        return value.Trim();
    }

    private static string? NormalizeOptionalText(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
