using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Efac.Tests.Api;

public sealed class ClientesApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient client;

    public ClientesApiTests(WebApplicationFactory<Program> factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task GetClientes_ReturnsOkWithSeedData()
    {
        var response = await client.GetAsync("/api/clientes");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadAsStringAsync();
        json.Should().Contain("1020304050");
        json.Should().Contain("Efac Demo S.A.S.");
    }

    [Fact]
    public async Task CreateCliente_WithDuplicateNit_ReturnsConflict()
    {
        var request = new
        {
            tipoPersona = 1,
            nit = "1020304050",
            nombres = "Duplicado",
            apellidos = "QA",
            razonSocial = (string?)null,
            fechaNacimiento = "1990-01-01",
            email = "duplicado.qa@example.com",
            telefono = "3009999999",
            direccion = "Calle 99 # 1-2",
            ciudadCodigoMunicipio = "11001",
            responsabilidadFiscal = 2
        };

        var response = await client.PostAsJsonAsync("/api/clientes", request);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var json = await response.Content.ReadAsStringAsync();
        json.Should().Contain("Ya existe un cliente registrado con el NIT indicado");
    }
}
