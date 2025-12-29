using System;

namespace GestaoClientes.Domain.Common
{
    public abstract class Entity
    {
        public virtual int Id { get; protected set; }
        public virtual DateTime DataCriacao { get; protected set; }
        public virtual DateTime? DataAtualizacao { get; protected set; }

        protected Entity()
        {
            DataCriacao = DateTime.UtcNow;
        }

        protected void MarcarComoAtualizado()
        {
            DataAtualizacao = DateTime.UtcNow;
        }
    }
}