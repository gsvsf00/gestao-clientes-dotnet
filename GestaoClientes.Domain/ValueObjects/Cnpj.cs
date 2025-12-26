using GestaoClientes.Domain.Exceptions;
using System;
using System.Text.RegularExpressions;

namespace GestaoClientes.Domain.ValueObjects
{
    public class Cnpj : IEquatable<Cnpj>
    {
        public string Numero { get; }
        private static readonly Regex _somenteNumeros = new Regex(@"[^\d]", RegexOptions.Compiled);

        public Cnpj(string numero)
        {
            if (string.IsNullOrWhiteSpace(numero))
                throw new DomainException("CNPJ não pode ser vazio ou nulo.");

            var cnpjLimpo = Limpar(numero);
            
            if (!Validar(cnpjLimpo))
                throw new DomainException($"CNPJ '{numero}' é inválido.");

            Numero = cnpjLimpo;
        }

        public static bool TryCreate(string numero, out Cnpj cnpj)
        {
            cnpj = null;
            
            if (string.IsNullOrWhiteSpace(numero))
                return false;

            try
            {
                var cnpjLimpo = Limpar(numero);
                if (!Validar(cnpjLimpo))
                    return false;

                cnpj = new Cnpj(cnpjLimpo);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static string Limpar(string cnpj) => _somenteNumeros.Replace(cnpj, "");

        private static bool Validar(string cnpj)
        {
            if (cnpj.Length != 14)
                return false;

            if (cnpj.All(c => c == cnpj[0]))
                return false;

            int[] multiplicador1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            string tempCnpj = cnpj.Substring(0, 12);
            int soma = 0;

            for (int i = 0; i < 12; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador1[i];

            int resto = soma % 11;
            int digito1 = resto < 2 ? 0 : 11 - resto;

            tempCnpj += digito1.ToString();

            int[] multiplicador2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            soma = 0;

            for (int i = 0; i < 13; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador2[i];

            resto = soma % 11;
            int digito2 = resto < 2 ? 0 : 11 - resto;

            return cnpj.EndsWith(digito1.ToString() + digito2.ToString());
        }

        public override string ToString() => 
            Convert.ToUInt64(Numero).ToString(@"00\.000\.000\/0000\-00");

        public string ToString(string format)
        {
            return format?.ToUpper() switch
            {
                "F" => ToString(),
                "N" => Numero,
                _ => ToString()
            };
        }

        public bool Equals(Cnpj other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Numero == other.Numero;
        }

        public override bool Equals(object obj) => 
            Equals(obj as Cnpj);

        public override int GetHashCode() => 
            Numero.GetHashCode();

        public static bool operator ==(Cnpj left, Cnpj right) => 
            Equals(left, right);

        public static bool operator !=(Cnpj left, Cnpj right) => 
            !Equals(left, right);

        public static implicit operator string(Cnpj cnpj) => cnpj?.Numero;
    }
}