using GestaoClientes.Application.Clientes.Queries;

namespace GestaoClientes.Application.Clientes.Queries;

public interface IObtemTodosClientesService
{
    Task<IEnumerable<ClienteDetalhesResult>> HandleAsync(
        ObtemTodosClientesQuery query, 
        CancellationToken cancellationToken = default);
}

