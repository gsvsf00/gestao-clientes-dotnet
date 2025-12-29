using GestaoClientes.Application.Common.Interfaces;
using GestaoClientes.Domain.Entities;
using GestaoClientes.Domain.ValueObjects;
using StackExchange.Redis;
using System.Text.Json;

namespace GestaoClientes.Infrastructure.Repositories;

// Decorator que adiciona cache Redis ao repositório de clientes.
// Implementa o padrão Decorator para envolver o repositório real.
public class ClienteRepositoryCacheDecorator : IClienteRepository
{
    private readonly IClienteRepository _repositorioReal;
    private readonly IDatabase _redis;
    private readonly TimeSpan _tempoDeExpiracao;
    private const string ChaveCachePrefix = "cliente:";

    public ClienteRepositoryCacheDecorator(
        IClienteRepository repositorioReal,
        IConnectionMultiplexer redis,
        TimeSpan? tempoDeExpiracao = null)
    {
        _repositorioReal = repositorioReal;
        _redis = redis.GetDatabase();
        _tempoDeExpiracao = tempoDeExpiracao ?? TimeSpan.FromMinutes(30);
    }

    // Busca um cliente pelo ID, consultando primeiro o cache Redis.
    // Se não encontrar no cache, consulta o repositório real e popula o cache.
    public async Task<Cliente?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var chaveCache = ObterChaveCache(id);
        
        // Tenta obter do cache (ignora erros do Redis)
        try
        {
        var valorCache = await _redis.StringGetAsync(chaveCache);
        
        if (valorCache.HasValue)
        {
                var clienteCache = DeserializarCliente(valorCache!);
                if (clienteCache != null)
                    return clienteCache;
            }
        }
        catch
        {
            // Ignora erros do Redis e continua para consultar o repositório real
        }

        // Consulta o repositório real
        var cliente = await _repositorioReal.GetByIdAsync(id, cancellationToken);
        
        // Popula o cache (ignora erros do Redis)
        if (cliente != null)
        {
            try
            {
            var valorSerializado = SerializarCliente(cliente);
            await _redis.StringSetAsync(chaveCache, valorSerializado, _tempoDeExpiracao);
            }
            catch
            {
                // Ignora erros do Redis para não bloquear a consulta
            }
        }

        return cliente;
    }

    // Busca um cliente pelo CNPJ.
    public async Task<Cliente?> GetByCnpjAsync(Cnpj cnpj, CancellationToken cancellationToken = default)
    {
        return await _repositorioReal.GetByCnpjAsync(cnpj, cancellationToken);
    }

    // Adiciona um novo cliente.
    public async Task<Cliente> AddAsync(Cliente cliente, CancellationToken cancellationToken = default)
    {
        var clienteAdicionado = await _repositorioReal.AddAsync(cliente, cancellationToken);
        
        // Popula o cache (ignora erros do Redis para não bloquear a operação)
        try
        {
        var chaveCache = ObterChaveCache(clienteAdicionado.Id);
        var valorSerializado = SerializarCliente(clienteAdicionado);
        await _redis.StringSetAsync(chaveCache, valorSerializado, _tempoDeExpiracao);
        }
        catch
        {
            // Ignora erros do Redis para não bloquear a persistência
        }
        
        return clienteAdicionado;
    }

    private static string ObterChaveCache(int id)
    {
        return $"{ChaveCachePrefix}{id}";
    }

    private static string SerializarCliente(Cliente cliente)
    {
        var dto = new
        {
            Id = cliente.Id,
            NomeFantasia = cliente.NomeFantasia,
            Cnpj = cliente.Cnpj.Numero,
            Ativo = cliente.Ativo,
            DataCriacao = cliente.DataCriacao,
            DataAtualizacao = cliente.DataAtualizacao
        };

        return JsonSerializer.Serialize(dto);
    }

    private static Cliente? DeserializarCliente(string json)
    {
        try
        {
            var dto = JsonSerializer.Deserialize<ClienteCacheDto>(json);
            if (dto == null) 
                return null;

            var cnpj = new Cnpj(dto.Cnpj);
            
            // Cria instância usando o construtor protegido via reflection
            // Nota: Reflection é necessário aqui pois a entidade tem construtor protegido para ORM
            var cliente = (Cliente)Activator.CreateInstance(
                typeof(Cliente),
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance,
                null,
                new object[] { dto.NomeFantasia, cnpj },
                null)!;

            // Restaura propriedades da entidade base e da classe
            var tipoCliente = typeof(Cliente);
            var tipoBase = tipoCliente.BaseType!;
            
            // Restaura Id
            var idProperty = tipoBase.GetProperty("Id", 
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            idProperty?.SetValue(cliente, dto.Id);

            // Restaura DataCriacao
            var dataCriacaoProperty = tipoBase.GetProperty("DataCriacao",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            dataCriacaoProperty?.SetValue(cliente, dto.DataCriacao);

            // Restaura DataAtualizacao
            var dataAtualizacaoProperty = tipoBase.GetProperty("DataAtualizacao",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            dataAtualizacaoProperty?.SetValue(cliente, dto.DataAtualizacao);

            // Restaura Ativo
            var ativoProperty = tipoCliente.GetProperty("Ativo",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            ativoProperty?.SetValue(cliente, dto.Ativo);

            return cliente;
        }
        catch
        {
            // Em caso de erro na deserialização, retorna null para forçar consulta ao repositório real
            return null;
        }
    }

    private class ClienteCacheDto
    {
        public int Id { get; set; }
        public string NomeFantasia { get; set; } = string.Empty;
        public string Cnpj { get; set; } = string.Empty;
        public bool Ativo { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataAtualizacao { get; set; }
    }
}

