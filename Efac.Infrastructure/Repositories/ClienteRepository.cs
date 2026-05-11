using Efac.Application.Abstractions;
using Efac.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Efac.Infrastructure.Persistence;

namespace Efac.Infrastructure.Repositories;

public sealed class ClienteRepository : IClienteRepository
{
    private readonly EfacDbContext dbContext;

    public ClienteRepository(EfacDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Cliente>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Clientes
            .AsNoTracking()
            .OrderBy(cliente => cliente.Nit)
            .ToListAsync(cancellationToken);
    }

    public async Task<Cliente?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Clientes.FirstOrDefaultAsync(cliente => cliente.Id == id, cancellationToken);
    }

    public async Task<Cliente?> GetByNitAsync(string nit, CancellationToken cancellationToken = default)
    {
        return await dbContext.Clientes.FirstOrDefaultAsync(cliente => cliente.Nit == nit, cancellationToken);
    }

    public async Task AddAsync(Cliente cliente, CancellationToken cancellationToken = default)
    {
        await dbContext.Clientes.AddAsync(cliente, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Cliente cliente, CancellationToken cancellationToken = default)
    {
        dbContext.Clientes.Update(cliente);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Cliente cliente, CancellationToken cancellationToken = default)
    {
        dbContext.Clientes.Remove(cliente);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
