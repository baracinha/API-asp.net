# Hotel API

API REST em ASP.NET Core 9 para uma plataforma de classificados imobiliários,
com gestão de propriedades, autenticação por JWT, chat entre utilizadores e
cache distribuído com Redis.

Projeto individual.

## Tech Stack

- **.NET 9** · ASP.NET Core Web API
- **Entity Framework Core 9** (Pomelo) + **MySQL**
- **JWT Bearer** para autenticação · **BCrypt** para hashing de passwords
- **Redis** (StackExchange) como cache distribuído
- **EPPlus** para exportação Excel · **NEST / Elasticsearch** para pesquisa
- **Swagger / OpenAPI** para documentação dos endpoints

## Arquitetura

Estrutura em camadas: `Controllers → Services → AppDbContext (EF Core)`,
com **DTOs** separados por feature (Auth, Message, Property) para isolar a API
do modelo de domínio. Uploads de imagens guardados em `wwwroot/images/properties/{id}/`.

## Endpoints principais

**Auth** — `/register`, `/login`
**Chat** — `/ListUsers`, `/GenerateChat`, `/BasicList`, `/InsertMessages`, `/ListMessages`
**Properties** — `/InsertProperty`, `/GetProperties`, `/GetMyProperties`,
`/GetPropertyById`, `/UpdateProperty`, `/DeleteProperty`, `/GetFilteredProperties`

## Modelo de dados

8 tabelas em MySQL (ver `database/schema.sql`):

- `utilizadores` — contas com `cargo` (utilizador / suporte / gerente / admin)
- `propriedades` — anúncios (tipo, status, negócio venda/aluguer, localização, áreas)
- `imagens_propriedades` — N imagens por propriedade
- `caracteristicas` + `propriedades_caracteristicas` — relação N:N (Garagem, Varanda, Jardim, Piscina, Mobilado)
- `conversas` + `mensagens` — chat entre utilizadores sobre uma propriedade
- `favoritos` — N:N utilizador ↔ propriedade
- `denuncias` — reports de utilizadores ou anúncios

Chaves estrangeiras com integridade referencial, índices nas FKs e `unique` no email.

## Como correr

```bash
# 1. Criar a base de dados
mysql -u root -p < database/schema.sql

# 2. Copiar e configurar o appsettings
cp appsettings.Example.json appsettings.json
# Editar: ConnectionStrings:Conn, Jwt:SecretKey, Jwt:Issuer, Jwt:Audience

# 3. Garantir que MySQL e Redis estão a correr (Redis em localhost:6379)

# 4. Correr a API
dotnet restore
dotnet run
```

Swagger disponível em `/swagger` em ambiente de desenvolvimento.
