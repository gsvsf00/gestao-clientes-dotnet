using GestaoClientes.Application.Clientes.Commands;
using GestaoClientes.Application.Common.Interfaces;
using GestaoClientes.Domain.Entities;
using GestaoClientes.Domain.ValueObjects;
using Moq;
using Xunit;
using ApplicationException = GestaoClientes.Application.Common.Exceptions.ApplicationException;

namespace GestaoClientes.Tests.Application.Clientes.Commands;

public class CriaClienteServiceTests
{
    private readonly Mock<IClienteRepository> _mockRepository;
    private readonly CriaClienteService _service;

    public CriaClienteServiceTests()
    {
        _mockRepository = new Mock<IClienteRepository>();
        _service = new CriaClienteService(_mockRepository.Object);
    }

    [Fact]
    public async Task HandleAsync_ComDadosValidos_DeveCriarCliente()
    {
        // Arrange
        var command = new CriaClienteCommand
        {
            NomeFantasia = "Empresa Teste LTDA",
            Cnpj = "01775634000189"
        };

        var cnpj = new Cnpj(command.Cnpj);
        
        _mockRepository
            .Setup(r => r.GetByCnpjAsync(It.IsAny<Cnpj>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Cliente?)null);

        _mockRepository
            .Setup(r => r.AddAsync(It.IsAny<Cliente>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Cliente c, CancellationToken ct) => c);

        // Act
        var resultado = await _service.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.NotNull(resultado);
        Assert.NotEqual(Guid.Empty, resultado.Id);
        Assert.Equal(command.NomeFantasia, resultado.NomeFantasia);
        Assert.Equal("01.775.634/0001-89", resultado.Cnpj);
        Assert.True(resultado.Ativo);
        Assert.NotNull(resultado.DataCriacao);

        _mockRepository.Verify(
            r => r.GetByCnpjAsync(It.IsAny<Cnpj>(), It.IsAny<CancellationToken>()), 
            Times.Once);
        
        _mockRepository.Verify(
            r => r.AddAsync(It.IsAny<Cliente>(), It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ComCnpjDuplicado_DeveLancarApplicationException()
    {
        // Arrange
        var command = new CriaClienteCommand
        {
            NomeFantasia = "Empresa Teste LTDA",
            Cnpj = "01775634000189"
        };

        var clienteExistente = new Cliente("Empresa Existente", new Cnpj("01775634000189"));
        
        _mockRepository
            .Setup(r => r.GetByCnpjAsync(It.IsAny<Cnpj>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(clienteExistente);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApplicationException>(
            () => _service.HandleAsync(command, CancellationToken.None)
        );

        Assert.Contains("Já existe um cliente cadastrado com o CNPJ", exception.Message);
        
        _mockRepository.Verify(
            r => r.GetByCnpjAsync(It.IsAny<Cnpj>(), It.IsAny<CancellationToken>()), 
            Times.Once);
        
        _mockRepository.Verify(
            r => r.AddAsync(It.IsAny<Cliente>(), It.IsAny<CancellationToken>()), 
            Times.Never);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    public async Task HandleAsync_ComNomeFantasiaInvalido_DeveLancarApplicationException(string nomeInvalido)
    {
        // Arrange
        var command = new CriaClienteCommand
        {
            NomeFantasia = nomeInvalido,
            Cnpj = "01775634000189"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ApplicationException>(
            () => _service.HandleAsync(command, CancellationToken.None)
        );

        _mockRepository.Verify(
            r => r.GetByCnpjAsync(It.IsAny<Cnpj>(), It.IsAny<CancellationToken>()), 
            Times.Never);
        
        _mockRepository.Verify(
            r => r.AddAsync(It.IsAny<Cliente>(), It.IsAny<CancellationToken>()), 
            Times.Never);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    [InlineData("12345678901234")] // CNPJ inválido
    [InlineData("00000000000000")] // CNPJ com dígitos iguais
    public async Task HandleAsync_ComCnpjInvalido_DeveLancarApplicationException(string cnpjInvalido)
    {
        // Arrange
        var command = new CriaClienteCommand
        {
            NomeFantasia = "Empresa Teste LTDA",
            Cnpj = cnpjInvalido
        };

        // Act & Assert
        await Assert.ThrowsAsync<ApplicationException>(
            () => _service.HandleAsync(command, CancellationToken.None)
        );

        _mockRepository.Verify(
            r => r.GetByCnpjAsync(It.IsAny<Cnpj>(), It.IsAny<CancellationToken>()), 
            Times.Never);
        
        _mockRepository.Verify(
            r => r.AddAsync(It.IsAny<Cliente>(), It.IsAny<CancellationToken>()), 
            Times.Never);
    }

    [Fact]
    public async Task HandleAsync_ComCnpjFormatado_DeveAceitarECriar()
    {
        // Arrange
        var command = new CriaClienteCommand
        {
            NomeFantasia = "Empresa Teste LTDA",
            Cnpj = "01.775.634/0001-89" // CNPJ formatado
        };

        _mockRepository
            .Setup(r => r.GetByCnpjAsync(It.IsAny<Cnpj>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Cliente?)null);

        _mockRepository
            .Setup(r => r.AddAsync(It.IsAny<Cliente>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Cliente c, CancellationToken ct) => c);

        // Act
        var resultado = await _service.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal("01.775.634/0001-89", resultado.Cnpj);

        _mockRepository.Verify(
            r => r.AddAsync(It.IsAny<Cliente>(), It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_DeveMarcarClienteComoAtivo()
    {
        // Arrange
        var command = new CriaClienteCommand
        {
            NomeFantasia = "Empresa Teste LTDA",
            Cnpj = "01775634000189"
        };

        Cliente? clienteCriado = null;

        _mockRepository
            .Setup(r => r.GetByCnpjAsync(It.IsAny<Cnpj>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Cliente?)null);

        _mockRepository
            .Setup(r => r.AddAsync(It.IsAny<Cliente>(), It.IsAny<CancellationToken>()))
            .Callback<Cliente, CancellationToken>((c, ct) => clienteCriado = c)
            .ReturnsAsync((Cliente c, CancellationToken ct) => c);

        // Act
        var resultado = await _service.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.NotNull(clienteCriado);
        Assert.True(clienteCriado.Ativo);
        Assert.True(resultado.Ativo);
    }
}

