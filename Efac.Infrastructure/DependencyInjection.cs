using Efac.Application.Abstractions;
using Efac.Infrastructure.Persistence;
using Efac.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Efac.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddDbContext<EfacDbContext>(options =>
            options.UseInMemoryDatabase("EfacClientesDb"));

        services.AddScoped<IClienteRepository, ClienteRepository>();

        return services;
    }
}
