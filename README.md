# SOS — Store Operations System

Do'konlar tarmog'ini boshqarish uchun mikroservis arxitekturasiga asoslangan backend platforma.

---

## Arxitektura

```
Client / Frontend
       │
       ▼
  API Gateway  (YARP, :5000)
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
        │            │              │
  PostgreSQL       Redis         RabbitMQ
    :5432          :6379    :5672 / UI :15672
```

Har bir servis Clean Architecture bo'yicha `Domain → Application → Infrastructure → API` qatlamlarida qurilgan.

**Shared kutubxonalar:**
- `Sos.Shared.Kernel` — `Entity<T>`, `AggregateRoot<T>`, `Result<T>`
- `Sos.Shared.Infrastructure` — `BaseDbContext`, `CurrentContext`, `ValidationBehavior`
- `Sos.Shared.Contracts` — servislararo integration events

---

## Servislar

| Servis     | Local  | Docker | DB              | Vazifa                                    |
|------------|--------|--------|-----------------|-------------------------------------------|
| ApiGateway | 61454  | 5000   | —               | YARP reverse proxy                        |
| Core       | 54830  | 5100   | SosCoreDb       | JWT auth, foydalanuvchilar, tashkilotlar  |
| Catalog    | 61916  | 5200   | SosCatalogDb    | Mahsulotlar, narxlar, ombor               |
| Commerce   | 54859  | 5300   | SosCommerceDb   | Kassa (POS), mijozlar (CRM), bonus        |
| Analytics  | 61489  | 5400   | SosAnalyticsDb  | Sotuv hisobotlari                         |

### Foydalanuvchi rollari

| Rol            | Huquqlar                                         |
|----------------|--------------------------------------------------|
| `SuperAdmin`   | To'liq kirish — barcha servislar va tashkilotlar |
| `StoreAdmin`   | Bitta do'konni boshqarish                        |
| `Cashier`      | Faqat kassa operatsiyalari (POS)                 |
| `Warehouseman` | Ombor va inventarizatsiya                        |
| `Analyst`      | Faqat hisobotlarni ko'rish                       |

---

## Ishga tushirish

### Talablar

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/9)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- `dotnet-ef` CLI: `dotnet tool install --global dotnet-ef`

### Variant A — Local

```bash
# 1. Infratuzilma
docker-compose up -d postgres redis rabbitmq

# 2. Servislar (har biri o'zi migrate qiladi)
dotnet run --project src/Services/Core/Sos.Core.API
dotnet run --project src/Services/Catalog/Sos.Catalog.API
dotnet run --project src/Services/Commerce/Sos.Commerce.API
dotnet run --project src/Services/Analytics/Sos.Analytics.API
dotnet run --project src/ApiGateway
```

Visual Studio: `sos-api.slnx` → *Multiple Startup Projects*.

### Variant B — Docker Compose

```bash
docker-compose up --build
```

---

## Konfiguratsiya

`appsettings.Development.json` (local) yoki `docker-compose.yml` environment bo'limi (Docker):

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

> ⚠️ `Jwt:Secret` ni production'da o'zgartiring.

---

## Boshlang'ich ma'lumotlar (Seed Data)

`Sos.Core.API` birinchi marta ishga tushganda avtomatik qo'shiladi:

| Maydon   | Qiymat            |
|----------|-------------------|
| UserName | `superAdmin`    |
| Parol    | `Admin@123456`    |
| Rol      | `SuperAdmin`      |


## API

Gateway: `http://localhost:5000` (Docker) / `http://localhost:61454` (local)

| Route                         | Servis   | Tavsif                       |
|-------------------------------|----------|------------------------------|
| `POST /api/auth/login`        | Core     | Tizimga kirish, JWT olish    |
| `POST /api/auth/register`     | Core     | Yangi foydalanuvchi          |
| `POST /api/auth/refresh`      | Core     | Token yangilash              |
| `GET  /api/organizations`     | Core     | Tashkilotlar ro'yxati        |
| `GET  /api/products`          | Catalog  | Mahsulotlar ro'yxati         |
| `GET  /api/stock`             | Catalog  | Ombor qoldiqlari             |
| `GET  /api/pricing`           | Catalog  | Narx qoidalari               |
| `POST /api/sales`             | Commerce | Yangi sotuv                  |
| `GET  /api/customers`         | Commerce | Mijozlar ro'yxati            |
| `GET  /api/analytics/summary` | Analytics| Sotuv hisoboti               |

**Swagger UI:** `http://localhost:{port}/swagger` — har bir servis uchun alohida.

**Autentifikatsiya:** `POST /api/auth/login` → `accessToken` → Swagger Authorize: `Bearer <token>`

---

## Migratsiyalar

```bash
#Yangi migration
dotnet ef migrations add InitialCreate --project src/Services/Core/Sos.Core.Infrastructure
dotnet ef migrations add InitialCreate --project src/Services/Catalog/Sos.Catalog.Infrastructure
dotnet ef migrations add InitialCreate --project src/Services/Commerce/Sos.Commerce.Infrastructure
dotnet ef migrations add InitialCreate --project src/Services/Analytics/Sos.Analytics.Infrastructure
```

Har bir Infrastructure loyihasida `IDesignTimeDbContextFactory` mavjud — `--startup-project` talab qilinmaydi.

Batafsil: [MIGRATIONS.md](MIGRATIONS.md)

---

## Loyiha strukturasi

```
sos-api/
├── src/
│   ├── ApiGateway/
│   └── Services/
│       ├── Core/        # Auth, Organizations
│       ├── Catalog/     # Products, Pricing, Inventory
│       ├── Commerce/    # POS, CRM, Loyalty
│       ├── Analytics/   # Reports
│       └── Shared/      # Kernel, Infrastructure, Contracts
├── docker/
│   └── postgres/init.sql
├── docker-compose.yml
└── sos-api.slnx
```

---

## Texnologiyalar

| Soha            | Texnologiya                      |
|-----------------|----------------------------------|
| Framework       | ASP.NET Core 9                   |
| ORM             | Entity Framework Core 9 + Npgsql |
| CQRS / Mediator | MediatR 12                       |
| Validation      | FluentValidation 11              |
| Auth            | JWT Bearer                       |
| API Gateway     | YARP 2.x                         |
| Logging         | Serilog                          |
| Database        | PostgreSQL 16                    |
| Cache           | Redis 7                          |
| Message Broker  | RabbitMQ 3.13                    |
| Containerization| Docker / Docker Compose          |

## Comment style

```
/// <summary>
/// Ball yig'ish. 
/// Начислить баллы.
/// </summary>
```