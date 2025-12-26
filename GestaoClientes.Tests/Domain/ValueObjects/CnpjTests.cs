using GestaoClientes.Domain.Exceptions;
using GestaoClientes.Domain.ValueObjects;
using Xunit;

namespace GestaoClientes.Tests.Domain.ValueObjects
{
    public class CnpjTests
    {
        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData(null)]
        public void Construtor_ComNumeroVazioOuNulo_DeveLancarDomainException(string numeroInvalido)
        {
            var exception = Assert.Throws<DomainException>(() => new Cnpj(numeroInvalido));
            Assert.Contains("não pode ser vazio", exception.Message);
        }

        [Theory]
        [InlineData("123")]
        [InlineData("1234567890123")]   // 13 dígitos
        [InlineData("123456789012345")] // 15 dígitos
        public void Construtor_ComNumeroComTamanhoInvalido_DeveLancarDomainException(string numeroInvalido)
        {
            Assert.Throws<DomainException>(() => new Cnpj(numeroInvalido));
        }

        [Theory]
        [InlineData("00000000000000")]
        [InlineData("11111111111111")]
        [InlineData("99999999999999")]
        public void Construtor_ComCnpjComDigitosIguais_DeveLancarDomainException(string cnpjInvalido)
        {
            Assert.Throws<DomainException>(() => new Cnpj(cnpjInvalido));
        }

        // CNPJs válidos (gerados via 4Devs)
        [Theory]
        [InlineData("01.775.634/0001-89")]
        [InlineData("01775634000189")]
        [InlineData("90.942.116/0001-32")]
        [InlineData("90942116000132")]
        [InlineData("24.477.676/0001-39")]
        [InlineData("24477676000139")]
        [InlineData("41.674.881/0001-03")]
        [InlineData("41674881000103")]
        public void Construtor_ComCnpjValido_DeveCriarInstancia(string input)
        {
            var cnpj = new Cnpj(input);

            Assert.NotNull(cnpj);
            Assert.Equal(14, cnpj.Numero.Length);
        }

        [Theory]
        [InlineData("01775634000189", "01.775.634/0001-89")]
        [InlineData("90942116000132", "90.942.116/0001-32")]
        [InlineData("24477676000139", "24.477.676/0001-39")]
        [InlineData("41674881000103", "41.674.881/0001-03")]
        public void ToString_DeveRetornarFormatadoCorretamente(string numero, string esperado)
        {
            var cnpj = new Cnpj(numero);

            var resultado = cnpj.ToString();

            Assert.Equal(esperado, resultado);
        }

        [Theory]
        [InlineData("01775634000189", "F", "01.775.634/0001-89")]
        [InlineData("01775634000189", "N", "01775634000189")]
        [InlineData("01775634000189", null, "01.775.634/0001-89")]
        [InlineData("01775634000189", "", "01.775.634/0001-89")]
        public void ToString_ComFormat_DeveRetornarCorretamente(string numero, string format, string esperado)
        {
            var cnpj = new Cnpj(numero);

            var resultado = cnpj.ToString(format);

            Assert.Equal(esperado, resultado);
        }

        [Fact]
        public void Equals_ComMesmoNumero_DeveRetornarTrue()
        {
            var cnpj1 = new Cnpj("01.775.634/0001-89");
            var cnpj2 = new Cnpj("01775634000189");

            Assert.True(cnpj1.Equals(cnpj2));
            Assert.True(cnpj1 == cnpj2);
        }

        [Fact]
        public void Equals_ComNumerosDiferentes_DeveRetornarFalse()
        {
            var cnpj1 = new Cnpj("01.775.634/0001-89");
            var cnpj2 = new Cnpj("90.942.116/0001-32");

            Assert.False(cnpj1.Equals(cnpj2));
            Assert.True(cnpj1 != cnpj2);
        }

        [Theory]
        [InlineData("01.775.634/0001-89", true)]
        [InlineData("01775634000189", true)]
        [InlineData("01.775.634/0001-88", false)] // dígito inválido
        [InlineData("", false)]
        [InlineData("123", false)]
        public void TryCreate_DeveRetornarResultadoCorreto(string input, bool esperado)
        {
            var sucesso = Cnpj.TryCreate(input, out var cnpj);

            Assert.Equal(esperado, sucesso);

            if (esperado)
                Assert.NotNull(cnpj);
            else
                Assert.Null(cnpj);
        }

        [Fact]
        public void GetHashCode_DeveSerConsistente()
        {
            var cnpj1 = new Cnpj("01.775.634/0001-89");
            var cnpj2 = new Cnpj("01775634000189");

            Assert.Equal(cnpj1.GetHashCode(), cnpj2.GetHashCode());
        }
    }
}
