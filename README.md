# SOS — Store Operations System

Unified Platform for Retail Management — do'konlar tarmog'ini boshqarish uchun mikroservis arxitekturasiga asoslangan backend platforma.

---

## Arxitektura

```
Client / Frontend
       │
       ▼
  API Gateway  (YARP, :5000)
       │
  ┌────┴────────────────────────────────────┐
  │              Microservices              │
  ├─────────────┬───────────┬───────────────┤
  │  Identity   │  Catalog  │   Inventory   │
  │  61944      │  61916    │   61991       │
  ├─────────────┼───────────┼───────────────┤
  │     POS     │  Pricing  │     CRM       │
  │  62119      │  62146    │   61938       │
  ├─────────────┴───────────┴───────────────┤
  │       Loyalty :62111 | Analytics :61489  │
  └─────────────────────────────────────────┘
       │                   │
  PostgreSQL :5432      Redis :6379
```

**Har bir servis** Clean Architecture bo'yicha 4 qatlamdan iborat:
- `Domain` — entities, domain events, value objects
- `Application` — CQRS (MediatR), commands, queries, interfaces
- `Infrastructure` — EF Core, repositories, DbContext
- `API` — controllers, Swagger, middleware

**Umumiy komponentlar** (`Shared/`):
- `Sos.Shared.Kernel` — `Entity<T>`, `AggregateRoot<T>`, `Result<T>`, domain primitives
- `Sos.Shared.Infrastructure` — `BaseDbContext`, `CurrentUserService`, Redis cache
- `Sos.Shared.Contracts` — servislararo integration events

---

## Servislar

| Servis      | Port   | DB            | Maqsad |
|-------------|--------|---------------|--------|
| ApiGateway  | 61454  | —             | YARP reverse proxy, barcha so'rovlarni yo'naltiradi |
| Identity    | 61944  | IdentityDb    | JWT auth, register/login/refresh, foydalanuvchi rollari |
| Catalog     | 61916  | CatalogDb     | Mahsulotlar, kategoriyalar, SKU (kirim partiyalari), o'lchov birliklari |
| Inventory   | 61991  | InventoryDb   | Ombor qoldiqlari, kirim/chiqim, minimum zaxira ogohlantirish |
| POS         | 62119  | PosDb         | Kassa operatsiyalari — chek ochish, tovar qo'shish, to'lov |
| Pricing     | 62146  | PricingDb     | Narx qoidalari, chegirmalar, vaqtinchalik aksiyalar |
| CRM         | 61938  | CrmDb         | Mijozlar bazasi, aloqa ma'lumotlari |
| Loyalty     | 62111  | LoyaltyDb     | Bonus ball tizimi — yig'ish va sarflash |
| Analytics   | 61489  | AnalyticsDb   | Sotuv hisobotlari, daromad statistikasi, top mahsulotlar |

### Foydalanuvchi rollari

| Rol            | Huquqlar |
|----------------|----------|
| `SuperAdmin`   | To'liq kirish — barcha servislar va do'konlar |
| `StoreAdmin`   | Bitta do'konni boshqarish |
| `Cashier`      | Faqat kassa operatsiyalari (POS) |
| `Warehouseman` | Ombor va inventarizatsiya |
| `Analyst`      | Faqat hisobotlarni ko'rish |

---

## Ishga tushirish

### Talablar

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- `dotnet-ef` CLI tool

```bash
dotnet tool install --global dotnet-ef
```

### 1. Infratuzilmani ishga tushirish

```bash
docker-compose up -d postgres redis
```

### 2. Migrationlarni ishga tushirish

```bash
dotnet ef database update \
  --project src/Services/Identity/Sos.Identity.Infrastructure \
  --startup-project src/Services/Identity/Sos.Identity.API

dotnet ef database update \
  --project src/Services/Catalog/Sos.Catalog.Infrastructure \
  --startup-project src/Services/Catalog/Sos.Catalog.API

dotnet ef database update \
  --project src/Services/Inventory/Sos.Inventory.Infrastructure \
  --startup-project src/Services/Inventory/Sos.Inventory.API

dotnet ef database update \
  --project src/Services/POS/Sos.POS.Infrastructure \
  --startup-project src/Services/POS/Sos.POS.API

dotnet ef database update \
  --project src/Services/Pricing/Sos.Pricing.Infrastructure \
  --startup-project src/Services/Pricing/Sos.Pricing.API

dotnet ef database update \
  --project src/Services/CRM/Sos.CRM.Infrastructure \
  --startup-project src/Services/CRM/Sos.CRM.API

dotnet ef database update \
  --project src/Services/Loyalty/Sos.Loyalty.Infrastructure \
  --startup-project src/Services/Loyalty/Sos.Loyalty.API

dotnet ef database update \
  --project src/Services/Analytics/Sos.Analytics.Infrastructure \
  --startup-project src/Services/Analytics/Sos.Analytics.API
```

### 3. Servislarni ishga tushirish

**Yangi yordamchi skriptlar:**

Sos-api papkasiga o‘ting va quyidagilarni yozing:

```powershell
cd d:\Projects\МоиПроекты\sos\sos-api
run-all
```

Bu komandalar barcha servislarni alohida PowerShell oynasida ishga tushiradi.

Barcha servislarni to‘xtatish uchun:

```powershell
cd d:\Projects\МоиПроекты\sos\sos-api
stop-all
```

**Terminal orqali individual run:**

```bash
dotnet run --project src/Services/Identity/Sos.Identity.API
dotnet run --project src/Services/Catalog/Sos.Catalog.API
dotnet run --project src/Services/Inventory/Sos.Inventory.API
dotnet run --project src/Services/POS/Sos.POS.API
dotnet run --project src/Services/Pricing/Sos.Pricing.API
dotnet run --project src/Services/CRM/Sos.CRM.API
dotnet run --project src/Services/Loyalty/Sos.Loyalty.API
dotnet run --project src/Services/Analytics/Sos.Analytics.API
dotnet run --project src/ApiGateway
```

**Visual Studio:** `sos-api.slnx` → *Multiple Startup Projects* ni sozlang.

**Docker Compose (to'liq stack):**

```bash
docker-compose up --build
```

### 4. Konfiguratsiya

Har bir servisning `appsettings.Development.json` da:

```json
{
  "ConnectionStrings": {
    "Default": "Host=localhost;Database=<ServiceDb>;Username=postgres;Password=postgres",
    "Redis": "localhost:6379"
  },
  "Jwt": {
    "Secret": "your-super-secret-key-min-32-chars-long",
    "Issuer": "sos-identity",
    "Audience": "sos-services",
    "ExpiryMinutes": 60
  }
}
```

> ⚠️ `Jwt:Secret` ni production'da albatta o'zgartiring.

---

## API endpointlari

Gateway orqali `http://localhost:5000` ga kelgan so'rovlar:

| Prefix               | Servis    | Misol                                           |
|----------------------|-----------|-------------------------------------------------|
| `/api/identity/...`  | Identity  | `POST /api/identity/auth/login`                 |
| `/api/catalog/...`   | Catalog   | `GET /api/catalog/products/barcode/{barcode}`   |
| `/api/inventory/...` | Inventory | `GET /api/inventory/stock/{storeId}/{productId}`|
| `/api/pos/...`       | POS       | `POST /api/pos/sales`                           |
| `/api/pricing/...`   | Pricing   | `GET /api/pricing/rules/{productId}`            |
| `/api/crm/...`       | CRM       | `POST /api/crm/customers`                       |
| `/api/loyalty/...`   | Loyalty   | `GET /api/loyalty/accounts/{customerId}`        |
| `/api/analytics/...` | Analytics | `GET /api/analytics/sales/summary`              |

### Swagger UI (Development)

| Servis    | URL                                |
|-----------|------------------------------------|
| Identity  | http://localhost:61944/swagger     |
| Catalog   | http://localhost:61916/swagger     |
| Inventory | http://localhost:61991/swagger     |
| POS       | http://localhost:62119/swagger     |
| Pricing   | http://localhost:62146/swagger     |
| CRM       | http://localhost:61938/swagger     |
| Loyalty   | http://localhost:62111/swagger     |
| Analytics | http://localhost:61489/swagger     |

### Autentifikatsiya

1. `POST /api/identity/auth/login` → `accessToken` oling
2. Swagger → **Authorize** → `Bearer <accessToken>`

---

## Texnologiyalar

| Soha             | Texnologiya |
|------------------|-------------|
| Framework        | ASP.NET Core 8 |
| ORM              | Entity Framework Core 8 + Npgsql |
| CQRS / Mediator  | MediatR 12 |
| Validation       | FluentValidation 11 |
| Auth             | JWT Bearer |
| API Gateway      | YARP 2.2 |
| Caching          | StackExchange.Redis 2.8 |
| Logging          | Serilog |
| Database         | PostgreSQL 16 |
| Containerization | Docker / Docker Compose |

---

## Loyiha strukturasi

```
sos-api/
├── src/
│   ├── ApiGateway/
│   ├── Services/
│   │   ├── Identity/    (API · Application · Domain · Infrastructure)
│   │   ├── Catalog/
│   │   ├── Inventory/
│   │   ├── POS/
│   │   ├── Pricing/
│   │   ├── CRM/
│   │   ├── Loyalty/
│   │   └── Analytics/
│   └── Shared/
│       ├── Sos.Shared.Kernel/         # Domain primitives, Result<T>
│       ├── Sos.Shared.Infrastructure/ # BaseDbContext, Redis, CurrentUser
│       └── Sos.Shared.Contracts/      # Integration events
├── docker/postgres/init.sql
├── docker-compose.yml
├── MIGRATIONS.md
└── sos-api.slnx
```

---

## Health checks

```bash
curl http://localhost:61944/health   # Identity
curl http://localhost:61916/health   # Catalog
curl http://localhost:61991/health   # Inventory
curl http://localhost:62119/health   # POS
curl http://localhost:62146/health   # Pricing
curl http://localhost:61938/health   # CRM
curl http://localhost:62111/health   # Loyalty
curl http://localhost:61489/health   # Analytics
```
