namespace GestaoClientes.Application.Clientes.Commands;

public class CriaClienteCommandResult
{
    public Guid Id { get; set; }
    public string NomeFantasia { get; set; } = string.Empty;
    public string Cnpj { get; set; } = string.Empty;
    public bool Ativo { get; set; }
    public DateTime DataCriacao { get; set; }
}


