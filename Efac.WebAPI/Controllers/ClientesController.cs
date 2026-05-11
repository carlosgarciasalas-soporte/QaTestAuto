using Efac.Application.DTOs;
using Efac.Application.Exceptions;
using Efac.Application.UseCases.Clientes;
using Efac.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Efac.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ClientesController : ControllerBase
{
    private readonly ClienteService clienteService;

    public ClientesController(ClienteService clienteService)
    {
        this.clienteService = clienteService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ClienteResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ClienteResponseDto>>> GetAll(CancellationToken cancellationToken)
    {
        var clientes = await clienteService.GetAllAsync(cancellationToken);
        return Ok(clientes);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ClienteResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClienteResponseDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var cliente = await clienteService.GetByIdAsync(id, cancellationToken);
        return cliente is null ? NotFound(new { error = "El cliente no existe" }) : Ok(cliente);
    }

    [HttpGet("calcular-dv/{nit}")]
    [ProducesResponseType(typeof(DvResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<DvResponseDto> CalculateDv(string nit)
    {
        try
        {
            return Ok(clienteService.CalculateDv(nit));
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { error = exception.Message });
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(ClienteResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ClienteResponseDto>> Create(ClienteCreateDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var cliente = await clienteService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = cliente.Id }, cliente);
        }
        catch (ApplicationValidationException exception) when (IsDuplicateNit(exception))
        {
            return Conflict(new { error = exception.Message });
        }
        catch (Exception exception) when (IsBusinessException(exception))
        {
            return BadRequest(new { error = exception.Message });
        }
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ClienteResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ClienteResponseDto>> Update(Guid id, ClienteUpdateDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var cliente = await clienteService.UpdateAsync(id, dto, cancellationToken);
            return Ok(cliente);
        }
        catch (ApplicationValidationException exception) when (IsNotFound(exception))
        {
            return NotFound(new { error = exception.Message });
        }
        catch (ApplicationValidationException exception) when (IsDuplicateNit(exception))
        {
            return Conflict(new { error = exception.Message });
        }
        catch (Exception exception) when (IsBusinessException(exception))
        {
            return BadRequest(new { error = exception.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await clienteService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
        catch (ApplicationValidationException exception) when (IsNotFound(exception))
        {
            return NotFound(new { error = exception.Message });
        }
        catch (Exception exception) when (IsBusinessException(exception))
        {
            return BadRequest(new { error = exception.Message });
        }
    }

    private static bool IsBusinessException(Exception exception)
    {
        return exception is DomainValidationException or ApplicationValidationException or ArgumentException;
    }

    private static bool IsNotFound(ApplicationValidationException exception)
    {
        return exception.Message == "El cliente no existe";
    }

    private static bool IsDuplicateNit(ApplicationValidationException exception)
    {
        return exception.Message.Contains("NIT indicado", StringComparison.OrdinalIgnoreCase);
    }
}
