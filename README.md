# SOS — Store Operations System

Unified Platform for Retail Management — do'konlar tarmog'ini boshqarish uchun mikroservis arxitekturasiga asoslangan backend platforma.

---

## Arxitektura

```
Client / Frontend
       │
       ▼
  API Gateway  (YARP, :5000 / local :61454)
       │
  ┌────┴──────────────────────────────────────┐
  │               Microservices               │
  ├───────────────┬───────────┬───────────────┤
  │     Core      │  Catalog  │   Commerce    │
  │  auth, org    │ products  │  sales, crm   │
  │  :5100        │  :5200    │  :5300        │
  ├───────────────┴───────────┴───────────────┤
  │              Analytics :5400              │
  └───────────────────────────────────────────┘
            │                   │
      PostgreSQL :5432       Redis :6379
```

**Har bir servis** Clean Architecture bo'yicha 4 qatlamdan iborat:
- `Domain` — entities, domain events, value objects
- `Application` — CQRS (MediatR), commands, queries, interfaces
- `Infrastructure` — EF Core, repositories, DbContext, `IDesignTimeDbContextFactory`
- `API` — controllers, Swagger, middleware

**Umumiy komponentlar** (`Shared/`):
- `Sos.Shared.Kernel` — `Entity<T>`, `AggregateRoot<T>`, `Result<T>`, domain primitives
- `Sos.Shared.Infrastructure` — `BaseDbContext`, `CurrentContext`, ValidationBehavior
- `Sos.Shared.Contracts` — servislararo integration events

---

## Servislar

| Servis    | Local port | Docker port | DB              | Tarkib                                          |
|-----------|-----------|-------------|-----------------|------------------------------------------------|
| ApiGateway| 61454     | 5000        | —               | YARP reverse proxy                             |
| Core      | 54830     | 5100        | SosCoreDb       | JWT auth, foydalanuvchilar, tashkilotlar       |
| Catalog   | 61916     | 5200        | SosCatalogDb    | Mahsulotlar, kategoriyalar, SKU, narxlar, ombor|
| Commerce  | 54859     | 5300        | SosCommerceDb   | Kassa (POS), mijozlar (CRM), bonus (Loyalty)   |
| Analytics | 61489     | 5400        | SosAnalyticsDb  | Sotuv hisobotlari, statistika                  |

### Foydalanuvchi rollari

| Rol            | Huquqlar                                          |
|----------------|---------------------------------------------------|
| `SuperAdmin`   | To'liq kirish — barcha servislar va tashkilotlar  |
| `StoreAdmin`   | Bitta do'konni boshqarish                         |
| `Cashier`      | Faqat kassa operatsiyalari (POS)                  |
| `Warehouseman` | Ombor va inventarizatsiya                         |
| `Analyst`      | Faqat hisobotlarni ko'rish                        |

---

## Ishga tushirish

### Talablar

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- `dotnet-ef` CLI tool

```bash
dotnet tool install --global dotnet-ef
```

---

### Variant A — Local (dotnet run)

#### 1. Infratuzilmani ishga tushirish

```bash
docker-compose up -d postgres redis
```

#### 2. Servislarni ishga tushirish

Har bir servis o'z `Program.cs` da avtomatik migrate qiladi, qo'shimcha migration buyruqlari shart emas.

```bash
dotnet run --project src/Services/Core/Sos.Core.API
dotnet run --project src/Services/Catalog/Sos.Catalog.API
dotnet run --project src/Services/Commerce/Sos.Commerce.API
dotnet run --project src/Services/Analytics/Sos.Analytics.API
dotnet run --project src/ApiGateway
```

**Visual Studio:** `sos-api.slnx` → *Multiple Startup Projects* ni sozlang.

---

### Variant B — Docker Compose (to'liq stack)

```bash
docker-compose up --build
```

Barcha servislar, PostgreSQL va Redis birgalikda ishga tushadi. Migratsiyalar avtomatik bo'ladi.

```bash
# Faqat infra (local development uchun)
docker-compose up -d postgres redis

# Faqat ma'lum bir servisni restart qilish
docker-compose restart core-service
```

---

### Konfiguratsiya

**Local development:** har bir servisning `appsettings.Development.json` fayli orqali.

**Docker:** `docker-compose.yml` dagi `environment` bo'limi orqali.

```json
{
  "ConnectionStrings": {
    "Default": "Host=localhost;Database=<ServiceDb>;Username=postgres;Password=postgres"
  },
  "Jwt": {
    "Secret": "your-super-secret-key-min-32-chars-long",
    "Issuer": "Sos",
    "Audience": "Sos.Clients",
    "ExpiryMinutes": 60
  }
}
```

> `Jwt:Secret` ni production'da albatta o'zgartiring va xavfsiz joyda saqlang.

---

## API endpointlari

Gateway orqali `http://localhost:5000` (Docker) yoki `http://localhost:61454` (local):

| Route                         | Servis   | Tavsif                         |
|-------------------------------|----------|-------------------------------|
| `POST /api/auth/login`        | Core     | Tizimga kirish, JWT olish      |
| `POST /api/auth/register`     | Core     | Yangi foydalanuvchi ro'yxati   |
| `POST /api/auth/refresh`      | Core     | Token yangilash                |
| `GET  /api/organizations`     | Core     | Tashkilotlar ro'yxati          |
| `GET  /api/products`          | Catalog  | Mahsulotlar ro'yxati           |
| `GET  /api/products/{id}`     | Catalog  | Mahsulot ma'lumotlari          |
| `GET  /api/measurement-units` | Catalog  | O'lchov birliklari             |
| `GET  /api/pricing`           | Catalog  | Narx qoidalari                 |
| `GET  /api/stock`             | Catalog  | Ombor qoldiqlari               |
| `POST /api/sales`             | Commerce | Yangi sotuv (chek ochish)      |
| `GET  /api/customers`         | Commerce | Mijozlar ro'yxati              |
| `GET  /api/loyalty/{id}`      | Commerce | Bonus hisob ma'lumotlari       |
| `GET  /api/analytics/summary` | Analytics| Sotuv hisoboti                 |

### Swagger UI (Development)

| Servis    | URL                              |
|-----------|----------------------------------|
| Core      | http://localhost:5100/swagger    |
| Catalog   | http://localhost:5200/swagger    |
| Commerce  | http://localhost:5300/swagger    |
| Analytics | http://localhost:5400/swagger    |

### Autentifikatsiya

1. `POST /api/auth/login` → `accessToken` oling
2. Swagger → **Authorize** → `Bearer <accessToken>`

---

## Loyiha strukturasi

```
sos-api/
├── src/
│   ├── ApiGateway/
│   │   ├── appsettings.json        # YARP route konfiguratsiyasi
│   │   └── Dockerfile
│   ├── Services/
│   │   ├── Core/                   # Auth + Organizations
│   │   │   ├── Sos.Core.API/
│   │   │   ├── Sos.Core.Application/
│   │   │   ├── Sos.Core.Domain/
│   │   │   └── Sos.Core.Infrastructure/
│   │   ├── Catalog/                # Products + Pricing + Inventory
│   │   │   ├── Sos.Catalog.API/
│   │   │   ├── Sos.Catalog.Application/
│   │   │   ├── Sos.Catalog.Domain/
│   │   │   └── Sos.Catalog.Infrastructure/
│   │   ├── Commerce/               # POS + CRM + Loyalty
│   │   │   ├── Sos.Commerce.API/
│   │   │   ├── Sos.Commerce.Application/
│   │   │   ├── Sos.Commerce.Domain/
│   │   │   └── Sos.Commerce.Infrastructure/
│   │   └── Analytics/              # Hisobotlar
│   │       ├── Sos.Analytics.API/
│   │       ├── Sos.Analytics.Application/
│   │       ├── Sos.Analytics.Domain/
│   │       └── Sos.Analytics.Infrastructure/
│   └── Shared/
│       ├── Sos.Shared.Kernel/         # Domain primitives, Result<T>
│       ├── Sos.Shared.Infrastructure/ # BaseDbContext, CurrentContext
│       └── Sos.Shared.Contracts/      # Integration events
├── docker/
│   └── postgres/
│       └── init.sql                # DB yaratish skripti
├── docker-compose.yml
├── MIGRATIONS.md
└── sos-api.slnx
```

---

## Migratsiyalar

Har bir Infrastructure loyihasida `IDesignTimeDbContextFactory` mavjud — `--startup-project` talab qilinmaydi.

```bash
# Yangi migration qo'shish
dotnet ef migrations add <MigrationName> --project src/Services/Core/Sos.Core.Infrastructure
dotnet ef migrations add <MigrationName> --project src/Services/Catalog/Sos.Catalog.Infrastructure
dotnet ef migrations add <MigrationName> --project src/Services/Commerce/Sos.Commerce.Infrastructure
dotnet ef migrations add <MigrationName> --project src/Services/Analytics/Sos.Analytics.Infrastructure

# Ma'lumotlar bazasini yangilash (avtomatik migratsiya bo'lmasa)
dotnet ef database update --project src/Services/Core/Sos.Core.Infrastructure
dotnet ef database update --project src/Services/Catalog/Sos.Catalog.Infrastructure
dotnet ef database update --project src/Services/Commerce/Sos.Commerce.Infrastructure
dotnet ef database update --project src/Services/Analytics/Sos.Analytics.Infrastructure
```

Batafsil: [MIGRATIONS.md](MIGRATIONS.md)

---

## Health checks

```bash
curl http://localhost:5100/health   # Core
curl http://localhost:5200/health   # Catalog
curl http://localhost:5300/health   # Commerce
curl http://localhost:5400/health   # Analytics
```

---

## Texnologiyalar

| Soha             | Texnologiya                         |
|------------------|-------------------------------------|
| Framework        | ASP.NET Core 9                      |
| ORM              | Entity Framework Core 9 + Npgsql    |
| CQRS / Mediator  | MediatR 12                          |
| Validation       | FluentValidation 11                 |
| Auth             | JWT Bearer                          |
| API Gateway      | YARP 2.x                            |
| Logging          | Serilog                             |
| Database         | PostgreSQL 16                       |
| Containerization | Docker / Docker Compose             |
