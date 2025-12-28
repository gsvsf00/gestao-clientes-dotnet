using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using GestaoClientes.Domain.Entities;

namespace GestaoClientes.Infrastructure.Persistence.NHibernate.Mappings;

// Mapeamento da entidade Cliente para o NHibernate
public class ClienteMap : ClassMapping<Cliente>
{
    public ClienteMap()
    {
        Table("Clientes");

        Id(cliente => cliente.Id, mapeamento =>
        {
            mapeamento.Column("Id");
            mapeamento.Generator(Generators.Guid);
        });

        Property(cliente => cliente.NomeFantasia, mapeamento =>
        {
            mapeamento.Column("NomeFantasia");
            mapeamento.Length(200);
            mapeamento.NotNullable(true);
        });

        Property(cliente => cliente.Cnpj, mapeamento =>
        {
            mapeamento.Column("Cnpj");
            mapeamento.Length(14);
            mapeamento.NotNullable(true);
            mapeamento.Type<GestaoClientes.Infrastructure.Persistence.NHibernate.UserTypes.CnpjUserType>();
        });

        Property(cliente => cliente.Ativo, mapeamento =>
        {
            mapeamento.Column("Ativo");
            mapeamento.NotNullable(true);
        });

        Property(cliente => cliente.DataCriacao, mapeamento =>
        {
            mapeamento.Column("DataCriacao");
            mapeamento.NotNullable(true);
        });

        Property(cliente => cliente.DataAtualizacao, mapeamento =>
        {
            mapeamento.Column("DataAtualizacao");
            mapeamento.NotNullable(false);
        });
    }
}

