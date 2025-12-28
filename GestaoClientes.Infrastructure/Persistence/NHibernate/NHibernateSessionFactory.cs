using NHibernate;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Mapping.ByCode;
using System.Data.SQLite;
using System.Reflection;

namespace GestaoClientes.Infrastructure.Persistence.NHibernate;

// Factory para criação de ISessionFactory do NHibernate configurado com SQLite
public class NHibernateSessionFactory
{
    private static ISessionFactory? _fabricaDeSessao;
    private static readonly object _bloqueio = new object();

    // Cria uma ISessionFactory configurada com SQLite usando connection string fornecida
    public static ISessionFactory GetSessionFactory(string stringDeConexao)
    {
        if (_fabricaDeSessao == null)
        {
            lock (_bloqueio)
            {
                if (_fabricaDeSessao == null)
                {
                    var configuracao = new Configuration();
                    
                    configuracao.DataBaseIntegration(banco =>
                    {
                        banco.ConnectionString = stringDeConexao;
                        banco.Dialect<SQLiteDialect>();
                        banco.Driver<SQLite20Driver>();
                        banco.LogSqlInConsole = true;
                        banco.LogFormattedSql = true;
                    });

                    var mapeador = new ModelMapper();
                    mapeador.AddMapping<GestaoClientes.Infrastructure.Persistence.NHibernate.Mappings.ClienteMap>();
                    
                    var mapeamento = mapeador.CompileMappingForAllExplicitlyAddedEntities();
                    configuracao.AddMapping(mapeamento);

                    configuracao.SetProperty("hbm2ddl.auto", "update");

                    _fabricaDeSessao = configuracao.BuildSessionFactory();
                }
            }
        }

        return _fabricaDeSessao;
    }

    // Cria uma ISessionFactory configurada com SQLite em memória para testes
    public static ISessionFactory GetInMemorySessionFactory()
    {
        if (_fabricaDeSessao == null)
        {
            lock (_bloqueio)
            {
                if (_fabricaDeSessao == null)
                {
                    var configuracao = new Configuration();
                    
                    configuracao.DataBaseIntegration(banco =>
                    {
                        banco.ConnectionString = "Data Source=:memory:;Version=3;New=True;";
                        banco.Dialect<SQLiteDialect>();
                        banco.Driver<SQLite20Driver>();
                        banco.LogSqlInConsole = true;
                        banco.LogFormattedSql = true;
                    });

                    var mapeador = new ModelMapper();
                    mapeador.AddMapping<GestaoClientes.Infrastructure.Persistence.NHibernate.Mappings.ClienteMap>();
                    
                    var mapeamento = mapeador.CompileMappingForAllExplicitlyAddedEntities();
                    configuracao.AddMapping(mapeamento);

                    configuracao.SetProperty("hbm2ddl.auto", "create-drop");

                    _fabricaDeSessao = configuracao.BuildSessionFactory();
                }
            }
        }

        return _fabricaDeSessao;
    }
}

