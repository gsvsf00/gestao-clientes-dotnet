using GestaoClientes.Domain.Entities;
using GestaoClientes.Domain.ValueObjects;
using GestaoClientes.Domain.Exceptions;
using Xunit;

namespace GestaoClientes.Tests.Domain.Entities
{
    public class ClienteTests
    {
        // CNPJs VÁLIDOS para os testes
        private readonly Cnpj _cnpjPadrao = new Cnpj("01775634000189");     // 01.775.634/0001-89
        private readonly Cnpj _outroCnpjValido = new Cnpj("90942116000132"); // 90.942.116/0001-32

        #region Testes do Construtor

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData(null)]
        public void Construtor_ComNomeFantasiaInvalido_DeveLancarDomainException(string nomeInvalido)
        {
            // Arrange & Act & Assert
            var exception = Assert.Throws<DomainException>(
                () => new Cliente(nomeInvalido, _cnpjPadrao)
            );
            
            Assert.Contains("Nome fantasia", exception.Message);
        }

        [Fact]
        public void Construtor_ComCnpjNulo_DeveLancarDomainException()
        {
            // Arrange & Act & Assert
            var exception = Assert.Throws<DomainException>(
                () => new Cliente("Empresa Teste", null)
            );
            
            Assert.Contains("CNPJ não pode ser nulo", exception.Message);
        }

        [Fact]
        public void Construtor_ComDadosValidos_DeveCriarCliente()
        {
            // Arrange
            var nomeValido = "Empresa Teste LTDA";

            // Act
            var cliente = new Cliente(nomeValido, _cnpjPadrao);

            // Assert
            Assert.Equal(nomeValido, cliente.NomeFantasia);
            Assert.Equal(_cnpjPadrao, cliente.Cnpj); // Comparação por valor do Value Object
            Assert.True(cliente.Ativo);
            // Id será 0 até ser persistido pelo NHibernate (auto-increment)
            Assert.Equal(0, cliente.Id);
            Assert.NotNull(cliente.DataCriacao);
        }

        #endregion

        #region Testes do SetNomeFantasia

        [Fact]
        public void SetNomeFantasia_ComNomeValido_DeveAtualizar()
        {
            // Arrange
            var cliente = new Cliente("Nome Antigo", _cnpjPadrao);
            var novoNome = "Novo Nome Fantasia";

            // Act
            cliente.SetNomeFantasia(novoNome);

            // Assert
            Assert.Equal(novoNome, cliente.NomeFantasia);
        }

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData(null)]
        [InlineData("AB")] // Menos de 3 caracteres
        public void SetNomeFantasia_ComNomeInvalido_DeveLancarDomainException(string nomeInvalido)
        {
            // Arrange
            var cliente = new Cliente("Nome Válido", _cnpjPadrao);

            // Act & Assert
            Assert.Throws<DomainException>(() => cliente.SetNomeFantasia(nomeInvalido));
        }

        #endregion

        #region Testes do SetCnpj (APENAS Value Object)

        [Fact]
        public void SetCnpj_ComCnpjValido_DeveAtualizar()
        {
            // Arrange
            var cliente = new Cliente("Empresa Teste", _cnpjPadrao);

            // Act
            cliente.SetCnpj(_outroCnpjValido);

            // Assert
            Assert.Equal(_outroCnpjValido, cliente.Cnpj);
            Assert.Equal("90.942.116/0001-32", cliente.Cnpj.ToString());
        }

        [Fact]
        public void SetCnpj_ComCnpjNulo_DeveLancarDomainException()
        {
            // Arrange
            var cliente = new Cliente("Empresa Teste", _cnpjPadrao);

            // Act & Assert
            var exception = Assert.Throws<DomainException>(
                () => cliente.SetCnpj(null)
            );
            
            Assert.Contains("CNPJ não pode ser nulo", exception.Message);
        }

        [Fact]
        public void SetCnpj_ComMesmoCnpj_DeveManterIgual()
        {
            // Arrange
            var cliente = new Cliente("Empresa Teste", _cnpjPadrao);
            var mesmoCnpj = new Cnpj("01.775.634/0001-89"); // Mesmo valor, instância diferente

            // Act
            cliente.SetCnpj(mesmoCnpj);

            // Assert
            Assert.Equal(_cnpjPadrao, cliente.Cnpj); // Value Objects são iguais por valor
            Assert.Equal("01.775.634/0001-89", cliente.Cnpj.ToString());
        }

        #endregion

        #region Testes dos Métodos de Domínio

        [Fact]
        public void Ativar_ClienteDesativado_DeveTornarAtivo()
        {
            // Arrange
            var cliente = new Cliente("Empresa Teste", _cnpjPadrao);
            cliente.Desativar();
            Assert.False(cliente.Ativo); // Verifica que está desativado

            // Act
            cliente.Ativar();

            // Assert
            Assert.True(cliente.Ativo);
        }

        [Fact]
        public void Desativar_ClienteAtivo_DeveTornarInativo()
        {
            // Arrange
            var cliente = new Cliente("Empresa Teste", _cnpjPadrao);
            Assert.True(cliente.Ativo); // Verifica que está ativo

            // Act
            cliente.Desativar();

            // Assert
            Assert.False(cliente.Ativo);
        }

        [Fact]
        public void AlterarCnpj_ComCnpjValido_DeveAtualizar()
        {
            // Arrange
            var cliente = new Cliente("Empresa Teste", _cnpjPadrao);
            var novoCnpj = new Cnpj("24477676000139"); // 24.477.676/0001-39

            // Act
            cliente.AlterarCnpj(novoCnpj);

            // Assert
            Assert.Equal(novoCnpj, cliente.Cnpj);
            Assert.Equal("24.477.676/0001-39", cliente.Cnpj.ToString());
        }

        [Fact]
        public void AlterarCnpj_ComCnpjNulo_DeveLancarDomainException()
        {
            // Arrange
            var cliente = new Cliente("Empresa Teste", _cnpjPadrao);

            // Act & Assert
            var exception = Assert.Throws<DomainException>(
                () => cliente.AlterarCnpj(null)
            );
            
            Assert.Contains("CNPJ não pode ser nulo", exception.Message);
        }

        #endregion

        #region Testes de Igualdade do Value Object no Cliente

        [Fact]
        public void CnpjDoCliente_DeveCompararPorValor()
        {
            // Arrange
            var cliente1 = new Cliente("Empresa A", new Cnpj("01775634000189"));
            var cliente2 = new Cliente("Empresa B", new Cnpj("01.775.634/0001-89"));

            // Assert - Value Objects são iguais por valor, não por referência
            Assert.Equal(cliente1.Cnpj, cliente2.Cnpj);
            Assert.True(cliente1.Cnpj == cliente2.Cnpj);
        }

        [Fact]
        public void CnpjDoCliente_ComValoresDiferentes_DeveSerDiferente()
        {
            // Arrange
            var cliente1 = new Cliente("Empresa A", new Cnpj("01775634000189"));
            var cliente2 = new Cliente("Empresa B", new Cnpj("90942116000132"));

            // Assert
            Assert.NotEqual(cliente1.Cnpj, cliente2.Cnpj);
            Assert.True(cliente1.Cnpj != cliente2.Cnpj);
        }

        #endregion
    }
}