# EF Core Migrations — Yo'riqnoma

## Bir martalik sozlash

```bash
dotnet tool install --global dotnet-ef
```

## Har bir servis uchun migration yaratish

Quyidagi buyruqlarni `sos-api/` root papkasidan ishlatish mumkin.

### Identity

```bash
dotnet ef migrations add InitialCreate \
  --project src/Services/Identity/Sos.Identity.Infrastructure \
  --startup-project src/Services/Identity/Sos.Identity.API \
  --output-dir Migrations
```

### Catalog

```bash
dotnet ef migrations add InitialCreate \
  --project src/Services/Catalog/Sos.Catalog.Infrastructure \
  --startup-project src/Services/Catalog/Sos.Catalog.API \
  --output-dir Migrations
```

### POS

```bash
dotnet ef migrations add InitialCreate \
  --project src/Services/POS/Sos.POS.Infrastructure \
  --startup-project src/Services/POS/Sos.POS.API \
  --output-dir Migrations
```

### Inventory

```bash
dotnet ef migrations add InitialCreate \
  --project src/Services/Inventory/Sos.Inventory.Infrastructure \
  --startup-project src/Services/Inventory/Sos.Inventory.API \
  --output-dir Migrations
```

### Pricing

```bash
dotnet ef migrations add InitialCreate \
  --project src/Services/Pricing/Sos.Pricing.Infrastructure \
  --startup-project src/Services/Pricing/Sos.Pricing.API \
  --output-dir Migrations
```

### CRM

```bash
dotnet ef migrations add InitialCreate \
  --project src/Services/CRM/Sos.CRM.Infrastructure \
  --startup-project src/Services/CRM/Sos.CRM.API \
  --output-dir Migrations
```

### Loyalty

```bash
dotnet ef migrations add InitialCreate \
  --project src/Services/Loyalty/Sos.Loyalty.Infrastructure \
  --startup-project src/Services/Loyalty/Sos.Loyalty.API \
  --output-dir Migrations
```

### Analytics

```bash
dotnet ef migrations add InitialCreate \
  --project src/Services/Analytics/Sos.Analytics.Infrastructure \
  --startup-project src/Services/Analytics/Sos.Analytics.API \
  --output-dir Migrations
```

---

## Database yaratish va migrate qilish

PostgreSQL ishlab turganida (yoki `docker-compose up -d postgres`):

```bash
dotnet ef database update \
  --project src/Services/Identity/Sos.Identity.Infrastructure \
  --startup-project src/Services/Identity/Sos.Identity.API

dotnet ef database update \
  --project src/Services/Catalog/Sos.Catalog.Infrastructure \
  --startup-project src/Services/Catalog/Sos.Catalog.API

dotnet ef database update \
  --project src/Services/POS/Sos.POS.Infrastructure \
  --startup-project src/Services/POS/Sos.POS.API

dotnet ef database update \
  --project src/Services/Inventory/Sos.Inventory.Infrastructure \
  --startup-project src/Services/Inventory/Sos.Inventory.API

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

---

## Auto-migrate (startup'da)

Pricing, CRM, Loyalty, Analytics servislarida Development rejimida avtomatik migrate yoqilgan.
Qolgan servislar uchun `Program.cs` ga `app.Run()` dan oldin qo'shish mumkin:

```csharp
using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<YourDbContext>();
await db.Database.MigrateAsync();
```

---

## Barcha servislarni ishga tushirish

```bash
# 1. Infra
docker-compose up -d postgres redis

# 2. Migration
# (yuqoridagi database update buyruqlari)

# 3. Servislar
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

Yoki Visual Studio'da `sos-api.slnx` oching — Multiple startup projects sozlang.
