using GestaoClientes.Application.Common.Interfaces;
using GestaoClientes.Application.Common.Exceptions;
using GestaoClientes.Domain.Entities;
using GestaoClientes.Domain.ValueObjects;

namespace GestaoClientes.Application.Clientes.Commands;

public class CriaClienteService : ICriaClienteService
{
    private readonly IClienteRepository _clienteRepository;

    public CriaClienteService(IClienteRepository clienteRepository)
    {
        _clienteRepository = clienteRepository;
    }

    public async Task<CriaClienteCommandResult> HandleAsync(
        CriaClienteCommand command, 
        CancellationToken cancellationToken = default)
    {
        // Validação de entrada
        if (string.IsNullOrWhiteSpace(command.NomeFantasia))
            throw new Common.Exceptions.ApplicationException("Nome fantasia é obrigatório.");

        if (string.IsNullOrWhiteSpace(command.Cnpj))
            throw new Common.Exceptions.ApplicationException("CNPJ é obrigatório.");

        // Criar Value Object CNPJ (validação automática)
        Cnpj cnpj;
        try
        {
            cnpj = new Cnpj(command.Cnpj);
        }
        catch (Domain.Exceptions.DomainException ex)
        {
            throw new Common.Exceptions.ApplicationException($"CNPJ inválido: {ex.Message}", ex);
        }

        // Verificar se já existe cliente com este CNPJ
        var clienteExistente = await _clienteRepository.GetByCnpjAsync(cnpj, cancellationToken);
        if (clienteExistente != null)
            throw new Common.Exceptions.ApplicationException($"Já existe um cliente cadastrado com o CNPJ {cnpj.ToString()}.");

        // Criar entidade Cliente
        var cliente = new Cliente(command.NomeFantasia.Trim(), cnpj);

        // Persistir
        await _clienteRepository.AddAsync(cliente, cancellationToken);

        // Retornar resultado
        return new CriaClienteCommandResult
        {
            Id = cliente.Id,
            NomeFantasia = cliente.NomeFantasia,
            Cnpj = cliente.Cnpj.ToString(),
            Ativo = cliente.Ativo,
            DataCriacao = cliente.DataCriacao
        };
    }
}

