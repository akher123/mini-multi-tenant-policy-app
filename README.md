# Mini Multi-Tenant Policy API

A minimal ASP.NET Core Web API demonstrating **tenant isolation** for a multi-tenant CRM-style policy system. Built with Clean Architecture, Carter minimal API modules, MediatR CQRS, FluentValidation, JWT auth with refresh tokens, and EF Core global query filters.

## Repository layout

```text
backend/
  MultiTenantPolicyApi.Domain/
  MultiTenantPolicyApi.Application/
  MultiTenantPolicyApi.Infrastructure/
  MultiTenantPolicyApi.Api/
  tests/MultiTenantPolicyApi.Tests/
frontend/
  MultiTenantPolicyApi.Client/    # Blazor WebAssembly
MultiTenantPolicyApp.slnx
```

## Quick start

**Prerequisites:** .NET 10 SDK (or .NET 9+), SQL Server LocalDB (included with Visual Studio / SQL Express).

```bash
dotnet restore MultiTenantPolicyApp.slnx
dotnet run --project backend/MultiTenantPolicyApi.Api
dotnet test MultiTenantPolicyApp.slnx
```

The API listens on `http://localhost:5105` (or the port shown in the console). Swagger UI is available in Development at `/swagger`.

## Blazor WebAssembly client

A standalone Blazor WASM UI calls the API with JWT authentication.

**1. Start the API** (terminal 1):

```bash
dotnet run --project backend/MultiTenantPolicyApi.Api
```

**2. Start the client** (terminal 2):

```bash
dotnet run --project frontend/MultiTenantPolicyApi.Client
```

Open `http://localhost:5216`, sign in with a seeded user, then use:

- **Customer Policy** — lookup by customer ID (try cross-tenant ID `cccccccc-cccc-cccc-cccc-cccccccccccc` as TenantA → 404)
- **Expiring Policies** — policies expiring within N days for your tenant
- **Policy Expiration** — exact expiration date from the database

Configure the API URL in [`frontend/MultiTenantPolicyApi.Client/wwwroot/appsettings.json`](frontend/MultiTenantPolicyApi.Client/wwwroot/appsettings.json) (`Api:BaseUrl`). CORS origins for the client are configured in the API [`backend/MultiTenantPolicyApi.Api/appsettings.json`](backend/MultiTenantPolicyApi.Api/appsettings.json).

## Try it (curl)

**1. Login as TenantA**

```bash
curl -X POST http://localhost:5105/api/auth/login \
  -H "Content-Type: application/json" \
  -d "{\"email\":\"admin@tenanta.local\",\"password\":\"Password123!\"}"
```

Copy `accessToken` from the response.

**2. Get a customer's policy**

```bash
curl http://localhost:5105/api/customers/aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa/policy \
  -H "Authorization: Bearer <accessToken>"
```

**3. List expiring policies**

```bash
curl "http://localhost:5105/api/policies/expiring?withinDays=30" \
  -H "Authorization: Bearer <accessToken>"
```

**4. Bonus — exact expiration date from DB**

```bash
curl http://localhost:5105/api/policies/POL-A-001/expiration \
  -H "Authorization: Bearer <accessToken>"
```

**Cross-tenant check:** Login as TenantA, then request TenantB's customer ID (`cccccccc-cccc-cccc-cccc-cccccccccccc`). Expect **404 Not Found** — not 403.

Seeded users: `admin@tenanta.local` / `admin@tenantb.local` (password: `Password123!`).

## Tenant isolation — how and why

Isolation uses three layers:

1. **Auth boundary** — At login, the user's `TenantId` is embedded in the JWT as a `tenant_id` claim. Clients cannot override tenant via headers or query params.
2. **Request scope** — `HttpTenantContext` reads `tenant_id` from the validated token on each request.
3. **Data boundary** — `AppDbContext` applies EF Core **global query filters** on `Customer` and `Policy`, automatically scoping every query to the current tenant.

**Why JWT + EF filters?** Defense in depth. JWT ensures the tenant context is authenticated and tamper-proof. EF global filters ensure that even if a future developer forgets a manual `WHERE TenantId = ...`, the database layer still filters. Cross-tenant access returns **404** (not 403) to avoid confirming that a resource exists in another tenant.

Key files for reviewers:
- `backend/MultiTenantPolicyApi.Infrastructure/Persistence/AppDbContext.cs` — global filters
- `backend/MultiTenantPolicyApi.Api/Services/HttpTenantContext.cs` — JWT → tenant
- `backend/tests/MultiTenantPolicyApi.Tests/GetCustomerPolicyQueryHandlerTests.cs` — cross-tenant unit test

## Project structure

| Layer | Project | Role |
|-------|---------|------|
| Domain | `backend/MultiTenantPolicyApi.Domain` | Entities |
| Application | `backend/MultiTenantPolicyApi.Application` | MediatR handlers, validators, interfaces |
| Infrastructure | `backend/MultiTenantPolicyApi.Infrastructure` | EF Core, auth, repositories |
| Api | `backend/MultiTenantPolicyApi.Api` | Carter endpoints, JWT wiring |
| Client | `frontend/MultiTenantPolicyApi.Client` | Blazor WebAssembly UI |

## Scaling to production

For real production workloads: use an external IdP (OAuth2/OIDC), store refresh tokens in a dedicated table with device binding, add SQL Server row-level security as a DB-level backup to EF filters, consider per-tenant databases or schemas for large clients, rotate signing keys, and add audit logging for cross-tenant access attempts.
