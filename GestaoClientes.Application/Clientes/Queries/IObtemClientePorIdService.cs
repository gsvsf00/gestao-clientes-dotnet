namespace GestaoClientes.Application.Clientes.Queries;

public interface IObtemClientePorIdService
{
    Task<ClienteDetalhesResult?> HandleAsync(
        ObtemClientePorIdQuery query, 
        CancellationToken cancellationToken = default);
}

