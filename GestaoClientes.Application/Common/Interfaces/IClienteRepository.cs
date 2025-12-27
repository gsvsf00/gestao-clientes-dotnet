using GestaoClientes.Domain.Entities;
using GestaoClientes.Domain.ValueObjects;

namespace GestaoClientes.Application.Common.Interfaces;

public interface IClienteRepository
{
    Task<Cliente?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Cliente?> GetByCnpjAsync(Cnpj cnpj, CancellationToken cancellationToken = default);
    Task<Cliente> AddAsync(Cliente cliente, CancellationToken cancellationToken = default);
}


