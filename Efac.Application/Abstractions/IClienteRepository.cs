using Efac.Domain.Entities;

namespace Efac.Application.Abstractions;

public interface IClienteRepository
{
    Task<IReadOnlyList<Cliente>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Cliente?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Cliente?> GetByNitAsync(string nit, CancellationToken cancellationToken = default);
    Task AddAsync(Cliente cliente, CancellationToken cancellationToken = default);
    Task UpdateAsync(Cliente cliente, CancellationToken cancellationToken = default);
    Task DeleteAsync(Cliente cliente, CancellationToken cancellationToken = default);
}
