using GestaoClientes.Application.Clientes.Commands;
using GestaoClientes.Application.Clientes.Queries;
using GestaoClientes.Application.Common.Interfaces;
using GestaoClientes.Infrastructure.Persistence.NHibernate;
using GestaoClientes.Infrastructure.Repositories;
using NHibernate;
using NHibernateSession = NHibernate.ISession;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Configuração do NHibernate
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Data Source=gestao_clientes.db;Version=3;";
var sessionFactory = NHibernateSessionFactory.GetSessionFactory(connectionString);

// Configuração do Redis
var redisConnectionString = builder.Configuration.GetConnectionString("Redis") 
    ?? "localhost:6379";
var redis = ConnectionMultiplexer.Connect(redisConnectionString);

// Configuração de serviços
builder.Services.AddSingleton<ISessionFactory>(sessionFactory);
builder.Services.AddScoped<NHibernateSession>(provider =>
{
    var factory = provider.GetRequiredService<ISessionFactory>();
    return factory.OpenSession();
});
builder.Services.AddSingleton<IConnectionMultiplexer>(redis);

// Registro dos repositórios (Decorator Pattern: Cache → NHibernate)
builder.Services.AddScoped<ClienteRepositoryNHibernate>();
builder.Services.AddScoped<IClienteRepository>(provider =>
{
    var session = provider.GetRequiredService<NHibernateSession>();
    var redisConnection = provider.GetRequiredService<IConnectionMultiplexer>();
    var repositorioNHibernate = new ClienteRepositoryNHibernate(session);
    return new ClienteRepositoryCacheDecorator(repositorioNHibernate, redisConnection);
});

// Registro dos serviços de aplicação
builder.Services.AddScoped<ICriaClienteService, CriaClienteService>();
builder.Services.AddScoped<IObtemClientePorIdService, ObtemClientePorIdService>();
builder.Services.AddScoped<IObtemTodosClientesService, ObtemTodosClientesService>();
builder.Services.AddScoped<IObtemTodosClientesService, ObtemTodosClientesService>();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
