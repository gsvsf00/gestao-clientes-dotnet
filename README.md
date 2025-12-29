# Gestão de Clientes - API .NET

API para gestão de clientes desenvolvida em .NET 9, seguindo os princípios de Clean Architecture, CQRS e Domain-Driven Design (DDD).

## Tecnologias

- **.NET 9** - Framework principal
- **NHibernate** - ORM para persistência
- **SQLite** - Banco de dados
- **Redis** - Cache para otimização de performance
- **xUnit** - Testes unitários
- **Moq** - Biblioteca para mocks

## Funcionalidades

- Cadastro de clientes com validação de CNPJ
- Consulta de cliente por ID
- Validação de CNPJ duplicado
- Cache com Redis para otimização de leituras
- Persistência com SQLite

## Como Rodar

### Pré-requisitos

- .NET 9 SDK
- Redis (opcional - sistema funciona sem cache se Redis estiver indisponível)

### Executar a API

```bash
cd GestaoClientes.API
dotnet run
```

A API estará disponível em `http://localhost:5112`

### Executar os Testes

```bash
dotnet test
```

## Endpoints

### POST /api/clientes
Cadastra um novo cliente.

**Request:**
```json
{
  "nomeFantasia": "Empresa Exemplo LTDA",
  "cnpj": "12.345.678/0001-95"
}
```

**Response:** `201 Created`
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "nomeFantasia": "Empresa Exemplo LTDA",
  "cnpj": "12.345.678/0001-95",
  "ativo": true
}
```

### GET /api/clientes/{id}
Consulta um cliente por ID.

**Response:** `200 OK`
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "nomeFantasia": "Empresa Exemplo LTDA",
  "cnpj": "12.345.678/0001-95",
  "ativo": true,
  "dataCriacao": "2025-12-29T10:30:00Z",
  "dataAtualizacao": "2025-12-29T10:30:00Z"
}
```

## Arquitetura

```
├── GestaoClientes.Domain/        # Entidades e regras de negócio
├── GestaoClientes.Application/   # Casos de uso (Commands/Queries)
├── GestaoClientes.Infrastructure/# Persistência e Cache
├── GestaoClientes.API/           # Controllers e configuração
└── GestaoClientes.Tests/         # Testes unitários
```

## Testes

O projeto possui 87 testes cobrindo:
- Entidades e Value Objects do domínio
- Handlers da camada de aplicação
- Cenários de sucesso e erro

---

**Desenvolvido como desafio técnico seguindo Clean Architecture, SOLID e DDD.**
