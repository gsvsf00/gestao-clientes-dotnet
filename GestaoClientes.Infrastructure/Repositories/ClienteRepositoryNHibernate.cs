using GestaoClientes.Application.Common.Interfaces;
using GestaoClientes.Domain.Entities;
using GestaoClientes.Domain.ValueObjects;
using NHibernate;
using NHibernate.Linq;

namespace GestaoClientes.Infrastructure.Repositories;

// Implementação do repositório de clientes utilizando NHibernate
public class ClienteRepositoryNHibernate : IClienteRepository
{
    private readonly ISession _sessao;

    public ClienteRepositoryNHibernate(ISession sessao)
    {
        _sessao = sessao;
    }

    // Busca um cliente pelo ID
    public async Task<Cliente?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _sessao.GetAsync<Cliente>(id, cancellationToken);
    }

    // Busca um cliente pelo CNPJ
    public async Task<Cliente?> GetByCnpjAsync(Cnpj cnpj, CancellationToken cancellationToken = default)
    {
        return await _sessao.Query<Cliente>()
            .Where(cliente => cliente.Cnpj.Numero == cnpj.Numero)
            .FirstOrDefaultAsync(cancellationToken);
    }

    // Adiciona um novo cliente ao banco de dados
    public async Task<Cliente> AddAsync(Cliente cliente, CancellationToken cancellationToken = default)
    {
        await _sessao.SaveAsync(cliente, cancellationToken);
        await _sessao.FlushAsync(cancellationToken);
        return cliente;
    }
}

