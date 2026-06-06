# EF Core Migrations — Yo'riqnoma

## Bir martalik sozlash

```bash
dotnet tool install --global dotnet-ef
```

## Migration yaratish

Har bir servisda `IDesignTimeDbContextFactory` mavjud, shuning uchun `--startup-project` talab qilinmaydi.
Quyidagi buyruqlarni `sos-api/` root papkasidan ishlating.

### Core

```bash
dotnet ef migrations add <MigrationName> \
  --project src/Services/Core/Sos.Core.Infrastructure
```

### Catalog

```bash
dotnet ef migrations add <MigrationName> \
  --project src/Services/Catalog/Sos.Catalog.Infrastructure
```

### Commerce

```bash
dotnet ef migrations add <MigrationName> \
  --project src/Services/Commerce/Sos.Commerce.Infrastructure
```

### Analytics

```bash
dotnet ef migrations add <MigrationName> \
  --project src/Services/Analytics/Sos.Analytics.Infrastructure
```

---

## Database yangilash (migrate qilish)

PostgreSQL ishlab turganida (yoki `docker-compose up -d postgres`):

```bash
dotnet ef database update --project src/Services/Core/Sos.Core.Infrastructure
dotnet ef database update --project src/Services/Catalog/Sos.Catalog.Infrastructure
dotnet ef database update --project src/Services/Commerce/Sos.Commerce.Infrastructure
dotnet ef database update --project src/Services/Analytics/Sos.Analytics.Infrastructure
```

> Yoki servislarni ishga tushirganda `Program.cs` ichidagi `db.Database.MigrateAsync()` orqali avtomatik migrate bo'ladi.

---

## Auto-migrate (startup'da)

Barcha servislarning `Program.cs` faylida `app.Run()` dan oldin avtomatik migration mavjud:

```csharp
using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<TDbContext>();
await db.Database.MigrateAsync();
```

---

## Migration o'chirish

```bash
dotnet ef migrations remove --project src/Services/<Service>/<Service>.Infrastructure
```
