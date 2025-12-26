using GestaoClientes.Domain.Exceptions;
using GestaoClientes.Domain.ValueObjects;
using Xunit;

namespace GestaoClientes.Tests.Domain.ValueObjects
{
    public class CnpjTests
    {
        // CNPJs VÁLIDOS gerados no 4devs:
        // 01.775.634/0001-89
        // 90.942.116/0001-32  
        // 24.477.676/0001-39
        // 41.674.881/0001-03

        #region Testes de Validação (Cenários de Erro)

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData(null)]
        public void Construtor_ComNumeroVazioOuNulo_DeveLancarDomainException(string numeroInvalido)
        {
            // Arrange & Act & Assert
            var exception = Assert.Throws<DomainException>(
                () => new Cnpj(numeroInvalido)
            );
            
            Assert.Contains("não pode ser vazio", exception.Message);
        }

        [Theory]
        [InlineData("123")]                     // Muito curto
        [InlineData("1234567890123")]          // 13 dígitos
        [InlineData("123456789012345")]        // 15 dígitos
        [InlineData("ABCDEFGHIJKLMN")]         // Letras
        [InlineData("12.345.678/9012-34")]     // Formato inválido
        public void Construtor_ComNumeroComTamanhoInvalido_DeveLancarDomainException(string numeroInvalido)
        {
            // Arrange & Act & Assert
            Assert.Throws<DomainException>(() => new Cnpj(numeroInvalido));
        }

        [Theory]
        [InlineData("00000000000000")]
        [InlineData("11111111111111")]
        [InlineData("22222222222222")]
        [InlineData("99999999999999")]
        public void Construtor_ComCnpjComDigitosIguais_DeveLancarDomainException(string cnpjInvalido)
        {
            // Arrange & Act & Assert
            Assert.Throws<DomainException>(() => new Cnpj(cnpjInvalido));
        }

        [Theory]
        [InlineData("12345678901234")]  // Dígitos verificadores inválidos
        [InlineData("98765432109876")]  // Dígitos verificadores inválidos
        public void Construtor_ComCnpjInvalido_DeveLancarDomainException(string cnpjInvalido)
        {
            // Arrange & Act & Assert
            Assert.Throws<DomainException>(() => new Cnpj(cnpjInvalido));
        }

        #endregion

        #region Testes de Sucesso (CNPJs Válidos)

        [Theory]
        [InlineData("01.775.634/0001-89", "01775634000189", "01.775.634/0001-89")]
        [InlineData("01775634000189", "01775634000189", "01.775.634/0001-89")]
        [InlineData("90.942.116/0001-32", "90942116000132", "90.942.116/0001-32")]
        [InlineData("90942116000132", "90942116000132", "90.942.116/0001-32")]
        [InlineData("24.477.676/0001-39", "24477676000139", "24.477.676/0001-39")]
        [InlineData("24477676000139", "24477676000139", "24.477.676/0001-39")]
        [InlineData("41.674.881/0001-03", "41674881000103", "41.674.881/0001-03")]
        [InlineData("41674881000103", "41674881000103", "41.674.881/0001-03")]
        public void Construtor_ComCnpjValido_DeveCriarInstancia(string input, string expectedNumero, string expectedFormatado)
        {
            // Arrange & Act
            var cnpj = new Cnpj(input);

            // Assert
            Assert.NotNull(cnpj);
            Assert.Equal(expectedNumero, cnpj.Numero);
            Assert.Equal(expectedFormatado, cnpj.ToString());
        }

        [Theory]
        [InlineData("01775634000189", "01.775.634/0001-89")]
        [InlineData("90942116000132", "90.942.116/0001-32")]
        [InlineData("24477676000139", "24.477.676/0001-39")]
        [InlineData("41674881000103", "41.674.881/0001-03")]
        public void ToString_DeveRetornarFormatadoCorretamente(string numero, string esperado)
        {
            // Arrange
            var cnpj = new Cnpj(numero);

            // Act
            var resultado = cnpj.ToString();

            // Assert
            Assert.Equal(esperado, resultado);
        }

        [Theory]
        [InlineData("01775634000189", "F", "01.775.634/0001-89")]
        [InlineData("01775634000189", "N", "01775634000189")]
        [InlineData("01775634000189", null, "01.775.634/0001-89")]
        [InlineData("01775634000189", "", "01.775.634/0001-89")]
        [InlineData("90942116000132", "F", "90.942.116/0001-32")]
        [InlineData("90942116000132", "N", "90942116000132")]
        public void ToString_ComFormat_DeveRetornarCorretamente(string numero, string format, string esperado)
        {
            // Arrange
            var cnpj = new Cnpj(numero);

            // Act
            var resultado = cnpj.ToString(format);

            // Assert
            Assert.Equal(esperado, resultado);
        }

        #endregion

        #region Testes de Igualdade

        [Fact]
        public void Equals_ComMesmoNumeroFormatadoDiferente_DeveRetornarTrue()
        {
            // Arrange
            var cnpj1 = new Cnpj("01775634000189");
            var cnpj2 = new Cnpj("01.775.634/0001-89");

            // Act & Assert
            Assert.True(cnpj1.Equals(cnpj2));
            Assert.True(cnpj1 == cnpj2);
            Assert.False(cnpj1 != cnpj2);
        }

        [Fact]
        public void Equals_ComNumerosDiferentes_DeveRetornarFalse()
        {
            // Arrange
            var cnpj1 = new Cnpj("01775634000189");
            var cnpj2 = new Cnpj("90942116000132");

            // Act & Assert
            Assert.False(cnpj1.Equals(cnpj2));
            Assert.False(cnpj1 == cnpj2);
            Assert.True(cnpj1 != cnpj2);
        }

        [Fact]
        public void Equals_ComNull_DeveRetornarFalse()
        {
            // Arrange
            var cnpj = new Cnpj("01775634000189");

            // Act & Assert
            Assert.False(cnpj.Equals(null));
        }

        [Fact]
        public void Equals_ComMesmoObjeto_DeveRetornarTrue()
        {
            // Arrange
            var cnpj = new Cnpj("01775634000189");

            // Act & Assert
            Assert.True(cnpj.Equals(cnpj));
        }

        #endregion

        #region Testes do Método TryCreate

        [Theory]
        [InlineData("01775634000189", true)]
        [InlineData("01.775.634/0001-89", true)]
        [InlineData("90942116000132", true)]
        [InlineData("90.942.116/0001-32", true)]
        [InlineData("12345678901234", false)] // Inválido
        [InlineData("", false)]
        [InlineData("123", false)]
        [InlineData("abcdefghijklmn", false)]
        public void TryCreate_DeveRetornarResultadoCorreto(string input, bool esperado)
        {
            // Act
            var sucesso = Cnpj.TryCreate(input, out var cnpj);

            // Assert
            Assert.Equal(esperado, sucesso);
            
            if (esperado)
            {
                Assert.NotNull(cnpj);
                // Verifica se o CNPJ foi criado corretamente
                Assert.Equal(new Cnpj(input).Numero, cnpj.Numero);
            }
            else
            {
                Assert.Null(cnpj);
            }
        }

        [Fact]
        public void TryCreate_ComCnpjValido_DeveCriarObjetoCorreto()
        {
            // Arrange
            var cnpjEsperado = new Cnpj("01775634000189");

            // Act
            var sucesso = Cnpj.TryCreate("01.775.634/0001-89", out var cnpj);

            // Assert
            Assert.True(sucesso);
            Assert.NotNull(cnpj);
            Assert.Equal(cnpjEsperado.Numero, cnpj.Numero);
            Assert.Equal(cnpjEsperado.ToString(), cnpj.ToString());
        }

        #endregion

        #region Testes de HashCode e Operadores

        [Fact]
        public void GetHashCode_DeveSerConsistenteParaMesmoNumero()
        {
            // Arrange
            var cnpj1 = new Cnpj("01775634000189");
            var cnpj2 = new Cnpj("01.775.634/0001-89");

            // Act & Assert
            Assert.Equal(cnpj1.GetHashCode(), cnpj2.GetHashCode());
        }

        [Fact]
        public void GetHashCode_DeveSerDiferenteParaNumerosDiferentes()
        {
            // Arrange
            var cnpj1 = new Cnpj("01775634000189");
            var cnpj2 = new Cnpj("90942116000132");

            // Act & Assert
            Assert.NotEqual(cnpj1.GetHashCode(), cnpj2.GetHashCode());
        }

        [Fact]
        public void OperadorIgualdade_ComNull_DeveFuncionar()
        {
            // Arrange
            Cnpj cnpj1 = null;
            Cnpj cnpj2 = null;

            // Act & Assert
            Assert.True(cnpj1 == cnpj2);
        }

        [Fact]
        public void OperadorIgualdade_UmNullOutroNao_DeveRetornarFalse()
        {
            // Arrange
            Cnpj cnpj1 = null;
            var cnpj2 = new Cnpj("01775634000189");

            // Act & Assert
            Assert.False(cnpj1 == cnpj2);
            Assert.False(cnpj2 == cnpj1);
        }

        #endregion

        #region Testes de Conversão Implícita

        [Fact]
        public void ConversaoImplicitaParaString_DeveRetornarNumero()
        {
            // Arrange
            var cnpj = new Cnpj("01775634000189");

            // Act
            string numero = cnpj;

            // Assert
            Assert.Equal("01775634000189", numero);
        }

        [Fact]
        public void ConversaoImplicitaComNull_DeveRetornarNull()
        {
            // Arrange
            Cnpj cnpj = null;

            // Act
            string numero = cnpj;

            // Assert
            Assert.Null(numero);
        }

        #endregion
    }
}