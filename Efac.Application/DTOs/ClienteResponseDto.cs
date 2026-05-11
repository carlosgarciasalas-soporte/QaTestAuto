using Efac.Domain.Enums;

namespace Efac.Application.DTOs;

public sealed record ClienteResponseDto(
    Guid Id,
    TipoPersona TipoPersona,
    string Nit,
    int Dv,
    string? Nombres,
    string? Apellidos,
    string? RazonSocial,
    string NombreCompleto,
    DateOnly? FechaNacimiento,
    string Email,
    string Telefono,
    string Direccion,
    string CiudadCodigoMunicipio,
    ResponsabilidadFiscal ResponsabilidadFiscal);
