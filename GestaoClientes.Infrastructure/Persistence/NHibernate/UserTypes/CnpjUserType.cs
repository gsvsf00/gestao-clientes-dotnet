using NHibernate;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;
using GestaoClientes.Domain.ValueObjects;
using System.Data;
using System.Data.Common;

namespace GestaoClientes.Infrastructure.Persistence.NHibernate.UserTypes;

// UserType customizado para persistir o Value Object Cnpj como VARCHAR no banco de dados
public class CnpjUserType : IUserType
{
    public SqlType[] SqlTypes => new[] { new SqlType(DbType.String) };

    public Type ReturnedType => typeof(Cnpj);

    public bool IsMutable => false;

    public object? Assemble(object? emCache, object? proprietario)
    {
        return emCache;
    }

    public object? DeepCopy(object? valor)
    {
        return valor;
    }

    public object? Disassemble(object? valor)
    {
        return valor;
    }

    public new bool Equals(object? x, object? y)
    {
        if (x == null && y == null) return true;
        if (x == null || y == null) return false;
        return x.Equals(y);
    }

    public int GetHashCode(object? x)
    {
        return x?.GetHashCode() ?? 0;
    }

    // Converte o valor do banco de dados (string) para o Value Object Cnpj
    public object? NullSafeGet(DbDataReader leitor, string[] nomes, ISessionImplementor sessao, object proprietario)
    {
        var valor = leitor[nomes[0]];
        if (valor == null || valor == DBNull.Value)
            return null;

        var cnpjComoString = valor.ToString();
        if (string.IsNullOrWhiteSpace(cnpjComoString))
            return null;

        try
        {
            return new Cnpj(cnpjComoString);
        }
        catch
        {
            return null;
        }
    }

    // Converte o Value Object Cnpj para string para persistir no banco de dados
    public void NullSafeSet(DbCommand comando, object? valor, int indice, ISessionImplementor sessao)
    {
        if (valor == null)
        {
            ((IDbDataParameter)comando.Parameters[indice]).Value = DBNull.Value;
        }
        else
        {
            var cnpj = (Cnpj)valor;
            ((IDbDataParameter)comando.Parameters[indice]).Value = cnpj.Numero;
        }
    }

    public object Replace(object original, object alvo, object proprietario)
    {
        return original;
    }
}

