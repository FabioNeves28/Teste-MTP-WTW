# Gestão de Pedidos

Sistema de gestão de pedidos para uma loja pequena: cadastro de produtos e clientes, criação de pedidos e controle de status com baixa e estorno de estoque.

- `Back/`: API em .NET 10 com ASP.NET Core, EF Core e SQL Server
- `Front/front-desafio/`: Angular 22 com Bootstrap

## Pré-requisitos

- .NET SDK 10
- Node.js 22+ e npm
- SQL Server (a configuração padrão usa o LocalDB, que vem com o Visual Studio)

## Como rodar

### Back-end

```bash
cd Back
dotnet tool restore
dotnet run --project src/GestaoPedidos.Api --launch-profile http
```

A API sobe em `http://localhost:5145` e o Swagger fica em `http://localhost:5145/swagger`.

Em ambiente de desenvolvimento a aplicação aplica as migrations e popula o banco com alguns produtos e clientes na inicialização, então não é preciso nenhum passo manual.

Para usar outro SQL Server, altere `ConnectionStrings:GestaoPedidos` em `src/GestaoPedidos.Api/appsettings.Development.json`.

### Front-end

```bash
cd Front/front-desafio
npm install
npm start
```

Acesse `http://localhost:4200`. A URL da API fica em `src/environments/environment.ts`.

### Testes

```bash
cd Back
dotnet test
```

Os testes de integração criam um banco temporário no LocalDB e o removem ao final.

## Banco de dados

O banco é criado pelas migrations do EF Core. Além da criação automática ao rodar em desenvolvimento, dá para criar de duas formas:

```bash
cd Back
dotnet ef database update -p src/GestaoPedidos.Infrastructure -s src/GestaoPedidos.Api
```

Ou executando o script `Back/database/script.sql`, gerado com `dotnet ef migrations script --idempotent`.

Os dados iniciais (5 produtos e 3 clientes) ficam em `DataSeeder` e só são inseridos se a tabela de produtos estiver vazia.

## Arquitetura

```
Back/src
├── GestaoPedidos.Domain          entidades, value objects, eventos, interfaces de repositório
├── GestaoPedidos.Application     commands/queries (MediatR), validações, handlers dos eventos
├── GestaoPedidos.Infrastructure  DbContext, mapeamentos, repositórios, migrations
└── GestaoPedidos.Api             controllers, tratamento de erros, configuração
```

O Domain não depende de nenhum outro projeto nem de bibliotecas externas.

No front, o código fica dividido em `core` (interceptor, toasts, tratamento de erro), `shared` (componentes e pipe reutilizáveis) e `features` (produtos, clientes e pedidos). Os componentes só chamam services tipados; nenhuma regra de negócio fica no front.

## Decisões técnicas

**Pedido como agregado.** Todas as regras ficam na entidade `Pedido`: não existe pedido sem item, a quantidade não pode passar do estoque, e as transições de status são validadas por uma tabela de transições permitidas. Qualquer violação lança `DomainException`.

**Domain Events para o estoque.** Ao confirmar, o `Pedido` só valida o estoque e registra um `PedidoConfirmadoEvent`. Quem baixa o estoque é o handler do evento. O cancelamento de um pedido confirmado funciona do mesmo jeito com `PedidoCanceladoEvent`. Assim o agregado Pedido não altera outro agregado diretamente.

**Eventos despachados na mesma transação.** O `SaveChangesAsync` do `DbContext` publica os eventos antes de gravar. Como os handlers alteram produtos já rastreados pelo mesmo contexto, o status do pedido e o estoque são salvos juntos. Se a baixa falhar, nada é gravado.

**Concorrência.** `Produto` tem uma coluna `RowVersion` (shadow property, para não expor detalhe de persistência no domínio). Se duas confirmações disputarem o mesmo estoque, a segunda recebe 409 e pode tentar de novo.

**Validação em duas camadas, sem duplicar regra.** O FluentValidation, via pipeline behavior do MediatR, valida formato (campos obrigatórios, tamanhos, quantidade positiva) e retorna 400 com os erros por campo. As regras de negócio ficam no domínio.

**Respostas de erro** seguem `ProblemDetails`:

| Situação | Status |
|---|---|
| Dados inválidos | 400, com `errors` por campo |
| Recurso não encontrado | 404 |
| Conflito de concorrência | 409 |
| Regra de negócio violada | 422 |
| Erro inesperado | 500, sem detalhes internos |

**Bibliotecas.** MediatR travado na 12.5 e FluentAssertions na 7.2, as últimas versões com licença livre.

## Premissas

- **.NET 10:** o teste pede .NET 9 ou superior. Usei o 10 porque era o SDK disponível.
- **Cadastro de clientes:** o enunciado pede seleção de cliente no pedido, mas não descreve o cadastro. Criei um simples, com nome, e-mail e CPF/CNPJ, sem permitir e-mail ou documento repetido.
- **Preço congelado:** o preço e o nome do produto são copiados para o item no momento do pedido. Alterar o produto depois não muda pedidos existentes.
- **Transições de status:** Criado → Confirmado ou Cancelado; Confirmado → Finalizado ou Cancelado. Cancelado e Finalizado são estados finais.
- **Estoque só muda na confirmação:** criar um pedido apenas valida se há estoque, sem reservar. Por isso o estoque é validado de novo ao confirmar. Cancelar um pedido que ainda não foi confirmado não mexe no estoque.
- **Itens repetidos:** o mesmo produto adicionado mais de uma vez vira um único item com a soma das quantidades.
- **Total calculado:** o total é sempre calculado a partir dos itens. O front mostra uma prévia, mas o valor que vale é o retornado pela API.
- **Itens só mudam enquanto o pedido está Criado.**
