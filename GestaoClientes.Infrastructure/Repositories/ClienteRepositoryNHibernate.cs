using GestaoClientes.Application.Common.Interfaces;
using GestaoClientes.Domain.Entities;
using GestaoClientes.Domain.ValueObjects;
using NHibernate;
using NHibernate.Linq;

namespace GestaoClientes.Infrastructure.Repositories;

/// <summary>
/// Implementação do repositório de clientes utilizando NHibernate.
/// </summary>
public class ClienteRepositoryNHibernate : IClienteRepository
{
    private readonly ISession _sessao;

    public ClienteRepositoryNHibernate(ISession sessao)
    {
        _sessao = sessao;
    }

    /// <summary>
    /// Busca um cliente pelo ID.
    /// </summary>
    public async Task<Cliente?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _sessao.GetAsync<Cliente>(id, cancellationToken);
    }

    /// <summary>
    /// Busca um cliente pelo CNPJ.
    /// </summary>
    public async Task<Cliente?> GetByCnpjAsync(Cnpj cnpj, CancellationToken cancellationToken = default)
    {
        return await _sessao.Query<Cliente>()
            .Where(cliente => cliente.Cnpj.Numero == cnpj.Numero)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Adiciona um novo cliente ao banco de dados.
    /// </summary>
    public async Task<Cliente> AddAsync(Cliente cliente, CancellationToken cancellationToken = default)
    {
        await _sessao.SaveAsync(cliente, cancellationToken);
        await _sessao.FlushAsync(cancellationToken);
        return cliente;
    }
}

