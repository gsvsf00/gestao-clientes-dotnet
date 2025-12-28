using System;

namespace GestaoClientes.Domain.Common
{
    public abstract class Entity
    {
        public virtual Guid Id { get; protected set; }
        public virtual DateTime DataCriacao { get; protected set; }
        public virtual DateTime? DataAtualizacao { get; protected set; }

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