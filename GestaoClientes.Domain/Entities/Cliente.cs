using GestaoClientes.Domain.Common;
using GestaoClientes.Domain.ValueObjects;
using GestaoClientes.Domain.Exceptions;
using System;

namespace GestaoClientes.Domain.Entities
{
    public class Cliente : Entity
    {
        public string NomeFantasia { get; private set; }
        public Cnpj Cnpj { get; private set; }
        public bool Ativo { get; private set; }

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

        public void SetNomeFantasia(string nomeFantasia)
        {
            if (string.IsNullOrWhiteSpace(nomeFantasia))
                throw new DomainException("Nome fantasia não pode ser vazio ou nulo.");

            if (nomeFantasia.Length < 3)
                throw new DomainException("Nome fantasia deve ter pelo menos 3 caracteres.");

            NomeFantasia = nomeFantasia.Trim();
            MarcarComoAtualizado();
        }

        public void SetCnpj(Cnpj cnpj)
        {
            Cnpj = cnpj ?? throw new DomainException("CNPJ não pode ser nulo.");
            MarcarComoAtualizado();
        }

        public void AlterarCnpj(Cnpj novoCnpj)
        {
            Cnpj = novoCnpj ?? throw new DomainException("CNPJ não pode ser nulo.");
            MarcarComoAtualizado();
        }

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