using Efac.Application.Abstractions;
using Efac.Application.DTOs;
using Efac.Application.Exceptions;
using Efac.Application.Mappings;
using Efac.Domain.Entities;
using Efac.Domain.Services;

namespace Efac.Application.UseCases.Clientes;

public sealed class ClienteService
{
    private readonly IClienteRepository repository;

    public ClienteService(IClienteRepository repository)
    {
        this.repository = repository;
    }

    public async Task<IReadOnlyList<ClienteResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var clientes = await repository.GetAllAsync(cancellationToken);
        return clientes.Select(cliente => cliente.ToResponseDto()).ToList();
    }

    public async Task<ClienteResponseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var cliente = await repository.GetByIdAsync(id, cancellationToken);
        return cliente?.ToResponseDto();
    }

    public async Task<ClienteResponseDto> CreateAsync(ClienteCreateDto dto, CancellationToken cancellationToken = default)
    {
        var normalizedNit = DianModulo11Calculator.NormalizeNit(dto.Nit);
        var existingCliente = await repository.GetByNitAsync(normalizedNit, cancellationToken);
        if (existingCliente is not null)
        {
            throw new ApplicationValidationException("Ya existe un cliente registrado con el NIT indicado");
        }

        var cliente = new Cliente(
            Guid.NewGuid(),
            dto.TipoPersona,
            dto.Nit,
            dto.Nombres,
            dto.Apellidos,
            dto.RazonSocial,
            dto.FechaNacimiento,
            dto.Email,
            dto.Telefono,
            dto.Direccion,
            dto.CiudadCodigoMunicipio,
            dto.ResponsabilidadFiscal);

        await repository.AddAsync(cliente, cancellationToken);
        return cliente.ToResponseDto();
    }

    public async Task<ClienteResponseDto> UpdateAsync(Guid id, ClienteUpdateDto dto, CancellationToken cancellationToken = default)
    {
        var cliente = await repository.GetByIdAsync(id, cancellationToken);
        if (cliente is null)
        {
            throw new ApplicationValidationException("El cliente no existe");
        }

        var normalizedNit = DianModulo11Calculator.NormalizeNit(dto.Nit);
        var existingCliente = await repository.GetByNitAsync(normalizedNit, cancellationToken);
        if (existingCliente is not null && existingCliente.Id != id)
        {
            throw new ApplicationValidationException("Ya existe otro cliente registrado con el NIT indicado");
        }

        cliente.Update(
            dto.TipoPersona,
            dto.Nit,
            dto.Nombres,
            dto.Apellidos,
            dto.RazonSocial,
            dto.FechaNacimiento,
            dto.Email,
            dto.Telefono,
            dto.Direccion,
            dto.CiudadCodigoMunicipio,
            dto.ResponsabilidadFiscal);

        await repository.UpdateAsync(cliente, cancellationToken);
        return cliente.ToResponseDto();
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var cliente = await repository.GetByIdAsync(id, cancellationToken);
        if (cliente is null)
        {
            throw new ApplicationValidationException("El cliente no existe");
        }

        await repository.DeleteAsync(cliente, cancellationToken);
    }

    public DvResponseDto CalculateDv(string nit)
    {
        var normalizedNit = DianModulo11Calculator.NormalizeNit(nit);
        var dv = DianModulo11Calculator.CalculateVerificationDigit(normalizedNit);
        return new DvResponseDto(normalizedNit, dv);
    }
}
