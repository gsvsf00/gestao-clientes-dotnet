using GestaoClientes.Domain.Common;
using GestaoClientes.Domain.Exceptions;
using System;

namespace GestaoClientes.Domain.Entities
{
    public class Cliente : Entity
    {
        public string NomeFantasia { get; private set; }
        public string CnpjNumero { get; private set; } // Será substituído pelo Value Object depois
        public bool Ativo { get; private set; }

        // Construtor privado para ORM
        protected Cliente() { }

        public Cliente(string nomeFantasia, string cnpjNumero)
        {
            SetNomeFantasia(nomeFantasia);
            SetCnpj(cnpjNumero);
            Ativo = true;
            MarcarComoAtualizado();
        }

        public void SetNomeFantasia(string nomeFantasia)
        {
            if (string.IsNullOrWhiteSpace(nomeFantasia))
                throw new DomainException("Nome fantasia não pode ser vazio ou nulo.");

            if (nomeFantasia.Length < 3)
                throw new DomainException("Nome fantasia deve ter pelo menos 3 caracteres.");

            NomeFantasia = nomeFantasia.Trim();
            MarcarComoAtualizado();
        }

        public void SetCnpj(string cnpjNumero)
        {
            if (string.IsNullOrWhiteSpace(cnpjNumero))
                throw new DomainException("CNPJ não pode ser vazio ou nulo.");

            // Validação básica - será substituída pelo Value Object
            var cnpjLimpo = cnpjNumero.Replace(".", "").Replace("/", "").Replace("-", "");
            if (cnpjLimpo.Length != 14)
                throw new DomainException("CNPJ deve ter 14 dígitos.");

            CnpjNumero = cnpjLimpo;
            MarcarComoAtualizado();
        }

        // Tarefa 2.3: Métodos de domínio
        public void Ativar()
        {
            Ativo = true;
            MarcarComoAtualizado();
        }

        public void Desativar()
        {
            Ativo = false;
            MarcarComoAtualizado();
        }
    }
}