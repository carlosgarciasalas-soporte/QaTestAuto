using Efac.Application.DTOs;
using Efac.Domain.Entities;

namespace Efac.Application.Mappings;

public static class ClienteMappings
{
    public static ClienteResponseDto ToResponseDto(this Cliente cliente)
    {
        return new ClienteResponseDto(
            cliente.Id,
            cliente.TipoPersona,
            cliente.Nit,
            cliente.Dv,
            cliente.Nombres,
            cliente.Apellidos,
            cliente.RazonSocial,
            cliente.NombreCompleto,
            cliente.FechaNacimiento,
            cliente.Email,
            cliente.Telefono,
            cliente.Direccion,
            cliente.CiudadCodigoMunicipio,
            cliente.ResponsabilidadFiscal);
    }
}
