using Efac.Domain.Enums;

namespace Efac.Application.DTOs;

public sealed record ClienteUpdateDto(
    TipoPersona TipoPersona,
    string Nit,
    string? Nombres,
    string? Apellidos,
    string? RazonSocial,
    DateOnly? FechaNacimiento,
    string Email,
    string Telefono,
    string Direccion,
    string CiudadCodigoMunicipio,
    ResponsabilidadFiscal ResponsabilidadFiscal);
