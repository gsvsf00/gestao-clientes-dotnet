using GestaoClientes.Domain.Entities;
using Xunit;

namespace GestaoClientes.Tests.Domain.Entities
{
    public class ClienteTests
    {
        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData(null)]
        public void Construtor_ComNomeFantasiaInvalido_DeveLancarExcecao(string nomeInvalido)
        {
            // Arrange & Act & Assert
            var exception = Assert.Throws<ArgumentException>(
                () => new Cliente(nomeInvalido, "12345678000195")
            );
            
            Assert.Contains("Nome fantasia", exception.Message);
        }

        [Fact]
        public void Construtor_ComNomeFantasiaValido_DeveCriarCliente()
        {
            // Arrange
            var nomeValido = "Empresa Teste LTDA";
            var cnpjValido = "12345678000195";

            // Act
            var cliente = new Cliente(nomeValido, cnpjValido);

            // Assert
            Assert.Equal(nomeValido, cliente.NomeFantasia);
            Assert.Equal(cnpjValido, cliente.CnpjNumero);
            Assert.True(cliente.Ativo);
            Assert.NotEqual(Guid.Empty, cliente.Id);
        }

        [Fact]
        public void SetNomeFantasia_ComNomeValido_DeveAtualizar()
        {
            // Arrange
            var cliente = new Cliente("Nome Antigo", "12345678000195");
            var novoNome = "Novo Nome Fantasia";

            // Act
            cliente.SetNomeFantasia(novoNome);

            // Assert
            Assert.Equal(novoNome, cliente.NomeFantasia);
        }

        [Fact]
        public void Ativar_ClienteDesativado_DeveTornarAtivo()
        {
            // Arrange
            var cliente = new Cliente("Empresa Teste", "12345678000195");
            cliente.Desativar();

            // Act
            cliente.Ativar();

            // Assert
            Assert.True(cliente.Ativo);
        }

        [Fact]
        public void Desativar_ClienteAtivo_DeveTornarInativo()
        {
            // Arrange
            var cliente = new Cliente("Empresa Teste", "12345678000195");

            // Act
            cliente.Desativar();

            // Assert
            Assert.False(cliente.Ativo);
        }
    }
}