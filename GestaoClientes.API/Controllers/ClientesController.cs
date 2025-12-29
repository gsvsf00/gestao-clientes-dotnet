using GestaoClientes.Application.Clientes.Commands;
using GestaoClientes.Application.Clientes.Queries;
using GestaoClientes.Application.Common.Exceptions;
using Microsoft.AspNetCore.Mvc;
using ApplicationException = GestaoClientes.Application.Common.Exceptions.ApplicationException;

namespace GestaoClientes.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientesController : ControllerBase
{
    private readonly ICriaClienteService _criaClienteService;
    private readonly IObtemClientePorIdService _obtemClientePorIdService;

    public ClientesController(
        ICriaClienteService criaClienteService,
        IObtemClientePorIdService obtemClientePorIdService)
    {
        _criaClienteService = criaClienteService;
        _obtemClientePorIdService = obtemClientePorIdService;
    }

    // Cria um novo cliente
    [HttpPost]
    [ProducesResponseType(typeof(CriaClienteCommandResult), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post(
        [FromBody] CriaClienteCommand command,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var resultado = await _criaClienteService.HandleAsync(command, cancellationToken);
            return CreatedAtAction(nameof(Get), new { id = resultado.Id }, resultado);
        }
        catch (ApplicationException ex)
        {
            return BadRequest(new { erro = ex.Message });
        }
    }

    // Obtém um cliente pelo ID
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ClienteDetalhesResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(
        int id,
        CancellationToken cancellationToken)
    {
        var query = new ObtemClientePorIdQuery { Id = id };
        var resultado = await _obtemClientePorIdService.HandleAsync(query, cancellationToken);

        if (resultado == null)
            return NotFound(new { erro = "Cliente não encontrado." });

        return Ok(resultado);
    }
}

