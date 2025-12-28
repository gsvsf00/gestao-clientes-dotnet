using GestaoClientes.Domain.Common;
using GestaoClientes.Domain.ValueObjects;
using GestaoClientes.Domain.Exceptions;
using System;

namespace GestaoClientes.Domain.Entities
{
    public class Cliente : Entity
    {
        public virtual string NomeFantasia { get; protected set; } = string.Empty;
        public virtual Cnpj Cnpj { get; protected set; } = null!;
        public virtual bool Ativo { get; protected set; }

        // Construtor privado para ORM
        protected Cliente() { }

        // Construtor principal
        public Cliente(string nomeFantasia, Cnpj cnpj)
        {
            SetNomeFantasia(nomeFantasia);
            SetCnpj(cnpj);
            Ativo = true;
            MarcarComoAtualizado();
        }

        public virtual void SetNomeFantasia(string nomeFantasia)
        {
            if (string.IsNullOrWhiteSpace(nomeFantasia))
                throw new DomainException("Nome fantasia não pode ser vazio ou nulo.");

            if (nomeFantasia.Length < 3)
                throw new DomainException("Nome fantasia deve ter pelo menos 3 caracteres.");

            NomeFantasia = nomeFantasia.Trim();
            MarcarComoAtualizado();
        }

        public virtual void SetCnpj(Cnpj cnpj)
        {
            Cnpj = cnpj ?? throw new DomainException("CNPJ não pode ser nulo.");
            MarcarComoAtualizado();
        }

        public virtual void AlterarCnpj(Cnpj novoCnpj)
        {
            Cnpj = novoCnpj ?? throw new DomainException("CNPJ não pode ser nulo.");
            MarcarComoAtualizado();
        }

        public virtual void Ativar()
        {
            Ativo = true;
            MarcarComoAtualizado();
        }

        public virtual void Desativar()
        {
            Ativo = false;
            MarcarComoAtualizado();
        }
    }
}