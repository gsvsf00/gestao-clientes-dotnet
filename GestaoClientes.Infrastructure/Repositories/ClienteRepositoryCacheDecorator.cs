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
    // Se não encontrar no cache (cache miss), consulta o repositório real e popula o cache.
    public async Task<Cliente?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var chaveCache = ObterChaveCache(id);
        
        // Tenta obter do cache primeiro
        var valorCache = await _redis.StringGetAsync(chaveCache);
        
        if (valorCache.HasValue)
        {
            // Cache hit - deserializa e retorna
            return DeserializarCliente(valorCache!);
        }

        // Cache miss - consulta o repositório real
        var cliente = await _repositorioReal.GetByIdAsync(id, cancellationToken);
        
        if (cliente != null)
        {
            // Popula o cache para próximas consultas
            var valorSerializado = SerializarCliente(cliente);
            await _redis.StringSetAsync(chaveCache, valorSerializado, _tempoDeExpiracao);
        }

        return cliente;
    }

    // Busca um cliente pelo CNPJ. Não utiliza cache nesta implementação.
    public async Task<Cliente?> GetByCnpjAsync(Cnpj cnpj, CancellationToken cancellationToken = default)
    {
        // Para GetByCnpj, não implementamos cache por enquanto
        return await _repositorioReal.GetByCnpjAsync(cnpj, cancellationToken);
    }

    // Adiciona um novo cliente. Popula o cache imediatamente após adicionar.
    public async Task<Cliente> AddAsync(Cliente cliente, CancellationToken cancellationToken = default)
    {
        var clienteAdicionado = await _repositorioReal.AddAsync(cliente, cancellationToken);
        
        // Popula o cache imediatamente após adicionar
        var chaveCache = ObterChaveCache(clienteAdicionado.Id);
        var valorSerializado = SerializarCliente(clienteAdicionado);
        await _redis.StringSetAsync(chaveCache, valorSerializado, _tempoDeExpiracao);
        
        return clienteAdicionado;
    }

    private static string ObterChaveCache(Guid id)
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
            if (dto == null) return null;

            var cnpj = new Cnpj(dto.Cnpj);
            
            // Cria instância usando o construtor protegido via reflexão
            var cliente = (Cliente)Activator.CreateInstance(
                typeof(Cliente),
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance,
                null,
                new object[] { dto.NomeFantasia, cnpj },
                null)!;

            // Define o ID usando reflexão
            var idProperty = typeof(Cliente).BaseType!.GetProperty("Id", 
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            idProperty?.SetValue(cliente, dto.Id);

            // Define DataCriacao
            var dataCriacaoProperty = typeof(Cliente).BaseType!.GetProperty("DataCriacao",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            dataCriacaoProperty?.SetValue(cliente, dto.DataCriacao);

            // Define DataAtualizacao
            var dataAtualizacaoProperty = typeof(Cliente).BaseType!.GetProperty("DataAtualizacao",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            dataAtualizacaoProperty?.SetValue(cliente, dto.DataAtualizacao);

            // Define Ativo
            var ativoProperty = typeof(Cliente).GetProperty("Ativo",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            ativoProperty?.SetValue(cliente, dto.Ativo);

            return cliente;
        }
        catch
        {
            return null;
        }
    }

    private class ClienteCacheDto
    {
        public Guid Id { get; set; }
        public string NomeFantasia { get; set; } = string.Empty;
        public string Cnpj { get; set; } = string.Empty;
        public bool Ativo { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataAtualizacao { get; set; }
    }
}

