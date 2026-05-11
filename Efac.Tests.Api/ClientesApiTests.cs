using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
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
        var request = CreateNaturalRequest("1020304050", "duplicado.qa@example.com");

        var response = await client.PostAsJsonAsync("/api/clientes", request);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var json = await response.Content.ReadAsStringAsync();
        json.Should().Contain("Ya existe un cliente registrado con el NIT indicado");
    }

    [Fact]
    public async Task GetClienteById_WhenClienteDoesNotExist_ReturnsNotFound()
    {
        var response = await client.GetAsync($"/api/clientes/{Guid.Empty}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var json = await response.Content.ReadAsStringAsync();
        json.Should().Contain("El cliente no existe");
    }

    [Fact]
    public async Task CalculateDv_WithFormattedNit_ReturnsNormalizedNitAndDv()
    {
        var response = await client.GetAsync("/api/clientes/calcular-dv/800.197.268-4");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        using var document = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
        document.RootElement.GetProperty("nit").GetString().Should().Be("8001972684");
        document.RootElement.GetProperty("dv").GetInt32().Should().Be(9);
    }

    [Fact]
    public async Task CalculateDv_WithInvalidNit_ReturnsBadRequest()
    {
        var response = await client.GetAsync("/api/clientes/calcular-dv/ABC");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var json = await response.Content.ReadAsStringAsync();
        json.Should().Contain("El NIT es obligatorio");
    }

    [Fact]
    public async Task CreateCliente_WithValidNaturalPerson_ReturnsCreated()
    {
        var request = CreateNaturalRequest("700100001", "natural.qa@example.com");

        var response = await client.PostAsJsonAsync("/api/clientes", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        using var document = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
        document.RootElement.GetProperty("id").GetGuid().Should().NotBeEmpty();
        document.RootElement.GetProperty("nit").GetString().Should().Be("700100001");
        document.RootElement.GetProperty("nombreCompleto").GetString().Should().Be("Cliente Natural");
        document.RootElement.GetProperty("razonSocial").ValueKind.Should().Be(JsonValueKind.Null);
    }

    [Fact]
    public async Task CreateCliente_WithUnderageNaturalPerson_ReturnsBadRequest()
    {
        var request = CreateNaturalRequest("700100002", "menor.qa@example.com", fechaNacimiento: "2010-01-01");

        var response = await client.PostAsJsonAsync("/api/clientes", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var json = await response.Content.ReadAsStringAsync();
        json.Should().Contain("El cliente debe ser mayor de edad");
    }

    [Fact]
    public async Task CreateCliente_WithNaturalPersonWithoutBirthDate_ReturnsBadRequest()
    {
        var request = CreateNaturalRequest("700100003", "sinfecha.qa@example.com", fechaNacimiento: null);

        var response = await client.PostAsJsonAsync("/api/clientes", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var json = await response.Content.ReadAsStringAsync();
        json.Should().Contain("La fecha de nacimiento es obligatoria para persona natural");
    }

    [Fact]
    public async Task CreateCliente_WithJuridicaWithoutRazonSocial_ReturnsBadRequest()
    {
        var request = CreateJuridicaRequest("900100001", "sinrazon.qa@example.com", razonSocial: null);

        var response = await client.PostAsJsonAsync("/api/clientes", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var json = await response.Content.ReadAsStringAsync();
        json.Should().Contain("La razon social es obligatoria para persona juridica");
    }

    [Fact]
    public async Task UpdateCliente_WithValidData_ReturnsOk()
    {
        var createResponse = await client.PostAsJsonAsync(
            "/api/clientes",
            CreateNaturalRequest("700100004", "actualizar.qa@example.com"));
        var clienteId = await ReadClienteIdAsync(createResponse);

        var updateRequest = CreateNaturalRequest("700100004", "actualizado.qa@example.com", nombres: "Cliente", apellidos: "Actualizado");
        var response = await client.PutAsJsonAsync($"/api/clientes/{clienteId}", updateRequest);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadAsStringAsync();
        json.Should().Contain("Cliente Actualizado");
        json.Should().Contain("actualizado.qa@example.com");
    }

    [Fact]
    public async Task DeleteCliente_WithExistingCliente_ReturnsNoContentAndRemovesCliente()
    {
        var createResponse = await client.PostAsJsonAsync(
            "/api/clientes",
            CreateJuridicaRequest("900100002", "eliminar.qa@example.com", "Empresa Eliminar S.A.S."));
        var clienteId = await ReadClienteIdAsync(createResponse);

        var deleteResponse = await client.DeleteAsync($"/api/clientes/{clienteId}");

        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await client.GetAsync($"/api/clientes/{clienteId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private static object CreateNaturalRequest(
        string nit,
        string email,
        string? fechaNacimiento = "1990-01-01",
        string nombres = "Cliente",
        string apellidos = "Natural")
    {
        return new
        {
            tipoPersona = 1,
            nit,
            nombres,
            apellidos,
            razonSocial = (string?)null,
            fechaNacimiento,
            email,
            telefono = "3009999999",
            direccion = "Calle 99 # 1-2",
            ciudadCodigoMunicipio = "11001",
            responsabilidadFiscal = 2
        };
    }

    private static object CreateJuridicaRequest(
        string nit,
        string email,
        string? razonSocial = "Empresa QA S.A.S.")
    {
        return new
        {
            tipoPersona = 2,
            nit,
            nombres = (string?)null,
            apellidos = (string?)null,
            razonSocial,
            fechaNacimiento = (string?)null,
            email,
            telefono = "6015550000",
            direccion = "Avenida 100 # 10-20",
            ciudadCodigoMunicipio = "11001",
            responsabilidadFiscal = 1
        };
    }

    private static async Task<Guid> ReadClienteIdAsync(HttpResponseMessage response)
    {
        response.EnsureSuccessStatusCode();
        using var document = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
        return document.RootElement.GetProperty("id").GetGuid();
    }
}
