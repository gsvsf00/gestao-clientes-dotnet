namespace GestaoClientes.Application.Clientes.Commands;

public interface ICriaClienteService
{
    Task<CriaClienteCommandResult> HandleAsync(
        CriaClienteCommand command, 
        CancellationToken cancellationToken = default);
}

