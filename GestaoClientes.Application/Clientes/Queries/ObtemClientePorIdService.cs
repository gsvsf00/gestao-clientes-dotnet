using GestaoClientes.Application.Common.Interfaces;
using GestaoClientes.Application.Common.Exceptions;

namespace GestaoClientes.Application.Clientes.Queries;

public class ObtemClientePorIdService
{
    private readonly IClienteRepository _clienteRepository;

    public ObtemClientePorIdService(IClienteRepository clienteRepository)
    {
        _clienteRepository = clienteRepository;
    }

    public async Task<ClienteDetalhesResult?> HandleAsync(
        ObtemClientePorIdQuery query, 
        CancellationToken cancellationToken = default)
    {
        var cliente = await _clienteRepository.GetByIdAsync(query.Id, cancellationToken);

        if (cliente == null)
            return null;

        return new ClienteDetalhesResult
        {
            Id = cliente.Id,
            NomeFantasia = cliente.NomeFantasia,
            Cnpj = cliente.Cnpj.ToString(),
            Ativo = cliente.Ativo,
            DataCriacao = cliente.DataCriacao,
            DataAtualizacao = cliente.DataAtualizacao
        };
    }
}


