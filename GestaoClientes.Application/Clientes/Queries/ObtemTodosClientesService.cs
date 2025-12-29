using GestaoClientes.Application.Common.Interfaces;

namespace GestaoClientes.Application.Clientes.Queries;

public class ObtemTodosClientesService : IObtemTodosClientesService
{
    private readonly IClienteRepository _clienteRepository;

    public ObtemTodosClientesService(IClienteRepository clienteRepository)
    {
        _clienteRepository = clienteRepository;
    }

    public async Task<IEnumerable<ClienteDetalhesResult>> HandleAsync(
        ObtemTodosClientesQuery query, 
        CancellationToken cancellationToken = default)
    {
        var clientes = await _clienteRepository.GetAllAsync(cancellationToken);

        return clientes.Select(cliente => new ClienteDetalhesResult
        {
            Id = cliente.Id,
            NomeFantasia = cliente.NomeFantasia,
            Cnpj = cliente.Cnpj.ToString(),
            Ativo = cliente.Ativo,
            DataCriacao = cliente.DataCriacao,
            DataAtualizacao = cliente.DataAtualizacao
        }).ToList();
    }
}

