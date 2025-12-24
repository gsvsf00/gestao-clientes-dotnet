using System;

namespace GestaoClientes.Domain.Common
{
    public abstract class Entity
    {
        public Guid Id { get; protected set; }
        public DateTime DataCriacao { get; protected set; }
        public DateTime? DataAtualizacao { get; protected set; }

        protected Entity()
        {
            Id = Guid.NewGuid();
            DataCriacao = DateTime.UtcNow;
        }

        protected void MarcarComoAtualizado()
        {
            DataAtualizacao = DateTime.UtcNow;
        }
    }
}