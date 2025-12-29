using GestaoClientes.Application.Clientes.Queries;
using GestaoClientes.Application.Common.Interfaces;
using GestaoClientes.Domain.Entities;
using GestaoClientes.Domain.ValueObjects;
using Moq;
using Xunit;

namespace GestaoClientes.Tests.Application.Clientes.Queries;

public class ObtemClientePorIdServiceTests
{
    private readonly Mock<IClienteRepository> _mockRepository;
    private readonly ObtemClientePorIdService _service;

    public ObtemClientePorIdServiceTests()
    {
        _mockRepository = new Mock<IClienteRepository>();
        _service = new ObtemClientePorIdService(_mockRepository.Object);
    }

    [Fact]
    public async Task HandleAsync_ComClienteExistente_DeveRetornarDetalhes()
    {
        // Arrange
        var clienteId = 1;
        var cliente = new Cliente("Empresa Teste LTDA", new Cnpj("01775634000189"));
        
        // Usar reflection para definir o Id (já que é protected set)
        var idProperty = typeof(Cliente).BaseType!.GetProperty("Id");
        idProperty!.SetValue(cliente, clienteId);

        var query = new ObtemClientePorIdQuery { Id = clienteId };

        _mockRepository
            .Setup(r => r.GetByIdAsync(clienteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cliente);

        // Act
        var resultado = await _service.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(clienteId, resultado.Id);
        Assert.Equal("Empresa Teste LTDA", resultado.NomeFantasia);
        Assert.Equal("01.775.634/0001-89", resultado.Cnpj);
        Assert.True(resultado.Ativo);
        Assert.NotNull(resultado.DataCriacao);

        _mockRepository.Verify(
            r => r.GetByIdAsync(clienteId, It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ComClienteInexistente_DeveRetornarNull()
    {
        // Arrange
        var clienteId = 999;
        var query = new ObtemClientePorIdQuery { Id = clienteId };

        _mockRepository
            .Setup(r => r.GetByIdAsync(clienteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Cliente?)null);

        // Act
        var resultado = await _service.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.Null(resultado);

        _mockRepository.Verify(
            r => r.GetByIdAsync(clienteId, It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ComClienteDesativado_DeveRetornarComAtivoFalse()
    {
        // Arrange
        var clienteId = 2;
        var cliente = new Cliente("Empresa Teste LTDA", new Cnpj("01775634000189"));
        cliente.Desativar();
        
        var idProperty = typeof(Cliente).BaseType!.GetProperty("Id");
        idProperty!.SetValue(cliente, clienteId);

        var query = new ObtemClientePorIdQuery { Id = clienteId };

        _mockRepository
            .Setup(r => r.GetByIdAsync(clienteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cliente);

        // Act
        var resultado = await _service.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.NotNull(resultado);
        Assert.False(resultado.Ativo);

        _mockRepository.Verify(
            r => r.GetByIdAsync(clienteId, It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_DeveFormatarCnpjCorretamente()
    {
        // Arrange
        var clienteId = 3;
        var cliente = new Cliente("Empresa Teste LTDA", new Cnpj("90942116000132"));
        
        var idProperty = typeof(Cliente).BaseType!.GetProperty("Id");
        idProperty!.SetValue(cliente, clienteId);

        var query = new ObtemClientePorIdQuery { Id = clienteId };

        _mockRepository
            .Setup(r => r.GetByIdAsync(clienteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cliente);

        // Act
        var resultado = await _service.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal("90.942.116/0001-32", resultado.Cnpj);
    }

    [Fact]
    public async Task HandleAsync_DeveIncluirDataCriacaoEAtualizacao()
    {
        // Arrange
        var clienteId = 4;
        var cliente = new Cliente("Empresa Teste LTDA", new Cnpj("01775634000189"));
        
        var idProperty = typeof(Cliente).BaseType!.GetProperty("Id");
        idProperty!.SetValue(cliente, clienteId);

        var query = new ObtemClientePorIdQuery { Id = clienteId };

        _mockRepository
            .Setup(r => r.GetByIdAsync(clienteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cliente);

        // Act
        var resultado = await _service.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.NotNull(resultado);
        Assert.NotNull(resultado.DataCriacao);
        Assert.NotNull(resultado.DataAtualizacao);
    }
}

