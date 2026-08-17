# Mini Multi-Tenant Policy API

A small ASP.NET Core API that keeps each tenant’s customer and policy data isolated. SQL Server LocalDB is used for storage. Tenant context comes from a JWT `tenant_id` claim, and EF Core global query filters apply that scope on every query.

## Run locally

Requires the .NET 10 SDK and SQL Server LocalDB.

```bash
dotnet restore MultiTenantPolicyApp.slnx
dotnet run --project backend/MultiTenantPolicyApi.Api
dotnet test MultiTenantPolicyApp.slnx
```

API: `http://localhost:5105` (Swagger at `/swagger` in Development).

Optional UI: `dotnet run --project frontend/MultiTenantPolicyApi.Client` → `http://localhost:5216`.

Seeded logins (password `Password123!`): `admin@tenanta.local`, `admin@tenantb.local`.

```bash
# 1. Login
curl -X POST http://localhost:5105/api/auth/login \
  -H "Content-Type: application/json" \
  -d "{\"email\":\"admin@tenanta.local\",\"password\":\"Password123!\"}"

# 2. Customer policy
curl http://localhost:5105/api/customers/aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa/policy \
  -H "Authorization: Bearer <accessToken>"

# 3. Expiring policies
curl "http://localhost:5105/api/policies/expiring?withinDays=30" \
  -H "Authorization: Bearer <accessToken>"

# 4. Bonus — exact expiration date from the database
curl http://localhost:5105/api/policies/POL-A-001/expiration \
  -H "Authorization: Bearer <accessToken>"
```

Cross-tenant check: as TenantA, request TenantB’s customer `cccccccc-cccc-cccc-cccc-cccccccccccc`. The API returns **404**, not 403, so it does not confirm that the other tenant’s record exists.

## Tenant isolation — how and why

Tenant is never taken from a header or query string. At login, `TenantId` is written into the JWT. `HttpTenantContext` reads that claim on each request. `AppDbContext` then applies global query filters on `Customer` and `Policy`:

```csharp
c => c.TenantId == _tenantContext.TenantId
p => p.TenantId == _tenantContext.TenantId
```

Handlers and repositories do not add a tenant `WHERE` clause. Isolation lives in the data layer, so a forgotten filter cannot leak another tenant’s rows. The unit test `GetCustomerPolicyQueryHandler_ReturnsNull_WhenCustomerIdBelongsToAnotherTenant` proves this: under TenantA’s context, TenantB’s `customerId` returns null.

JWT is used instead of a client-supplied tenant header because the claim is signed and cannot be swapped. Filters are used instead of per-query checks so every read is scoped, including the expiring-policies list and the bonus expiration lookup.

## Scaling to production

For real data: add SQL Server row-level security as a database-level backup, consider a database or schema per large tenant, use an external identity provider, persist and rotate refresh tokens, and audit denied cross-tenant lookups. The current shared-database + filter approach is enough for this exercise.
