using Efac.Application.UseCases.Clientes;
using Microsoft.Extensions.DependencyInjection;

namespace Efac.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ClienteService>();
        return services;
    }
}
