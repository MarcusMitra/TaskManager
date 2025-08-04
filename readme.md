
# TaskManager API

  

Este é um aplicativo Web Full Stack com backend em **ASP.NET Core** que consome e apresenta uma lista de tarefas, permitindo busca, visualização de detalhes, atualização e lógica adicional de negócios.

  

---

  

## Tecnologias

  

-  **.NET 8.0**

-  **ASP.NET Core Web API**

-  **Entity Framework Core** (SQLite)

-  **xUnit** (testes de integração)

-  **Moq** (mock de repositório)

  

## Visão Geral dos Endpoints

| Método  | Rota | Descrição |
|--|--|--|
| GET | `/todos` | Lista as tarefas com suporte a **paginação**`?page=1&pageSize=10`, filtro por **título**`?title=expedita` e **ordenação**`?sort=title&order=asc`. |
| GET | `/todos/{id}` | Retorna detalhes de uma tarefa por ID. |
| PUT | `/todos/{id}` | Atualiza o status `completed` de uma tarefa. |
| POST | `/sync` | Carrega tarefas do `https://jsonplaceholder.typicode.com/todos` e armazena no banco local. |

### Parâmetros de Consulta (`/todos`)

  

-  **page**: número da página (padrão: `1`).

-  **pageSize**: tamanho da página (padrão: `10`).

-  **title**: filtro parcial pelo campo título.

-  **sort**: campo de ordenação (`title`, `userId`, `completed`).

-  **order**: direção da ordenação (`asc`, `desc`).

  

## Regra de Negócio Adicional

  

Cada usuário (`userId`) só pode ter **até 5 tarefas incompletas**. Caso tente marcar uma tarefa como incompleta além desse limite, a API retorna **HTTP 400** com mensagem explicativa.

  

## Arquitetura & Organização

  

-  **Controllers**: recebem as requisições HTTP e delegam para a camada de aplicação.

-  **Application**: contém a lógica de casos de uso (`AssignmentAppService`), com validações, filtros, paginação e chamadas ao repositório.

-  **Domain**: modelos de domínio (`Assignment`), DTOs e interfaces de repositório.

-  **Infra.Data**: implementação do repositório usando EF Core (`TaskManagerContext`).

-  **Tests**: testes de integração com xUnit para validar filtros, paginação, regra de negócio e endpoints.

  

## Configuração do Banco de Dados

  

Por padrão, a aplicação está configurada para usar **SQLite** com o arquivo `database.db` no diretório raiz do projeto TaskManagerAPI. Para usar outro, edite a string de conexão em `appsettings.json`.

  

## Build e Execução

  

1. Clone o repositório:

```bash

git clone <url-do-repo> TaskManager

cd TaskManager/TaskManagerAPI

```

2. Restaure pacotes e faça build:

```bash

dotnet restore

dotnet build

```

3. Execute a API:

```bash

dotnet run --project TaskManagerAPI

```

A API estará disponível em `https://localhost:7025` (por padrão).

  

## Testes de Integração

  

Para executar os testes:

  

```bash

dotnet  test  TaskManager.Tests/TaskManager.Tests.csproj

```

  

Os testes cobrem:

  

- Filtros e paginação no endpoint GET `/todos`

- Ordenação ascendente e descendente

- Paginação correta

- Regra de negócio de limite de tarefas incompletas

- Comportamento dos endpoints PUT e GET

  

---


  

## Licença

  

Este projeto está licenciado sob a [MIT License](LICENSE).
