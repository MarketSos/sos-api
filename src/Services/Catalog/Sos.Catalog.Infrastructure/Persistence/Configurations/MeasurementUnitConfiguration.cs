using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sos.Catalog.Domain.Entities;

namespace Sos.Catalog.Infrastructure.Persistence.Configurations;

public class MeasurementUnitConfiguration : IEntityTypeConfiguration<MeasurementUnit>
{
    public void Configure(EntityTypeBuilder<MeasurementUnit> builder)
    {
        builder.HasKey(u => u.Id);

        builder.HasIndex(u => u.Code).IsUnique();

        builder.Property(u => u.Code).HasMaxLength(20).IsRequired();
        builder.Property(u => u.NameUz).HasMaxLength(100).IsRequired();
        builder.Property(u => u.NameRu).HasMaxLength(100).IsRequired();
        builder.Property(u => u.NameEn).HasMaxLength(100);
        builder.Property(u => u.NameUzKiril).HasMaxLength(100);

        builder.HasData(
            MeasurementUnit.Create(new Guid("00000000-0000-0000-0001-000000000001"), "dona",  "Dona",      "Штука",   "Piece", false),
            MeasurementUnit.Create(new Guid("00000000-0000-0000-0001-000000000002"), "kg",    "Kilogramm", "Кг",      "kg",    true),
            MeasurementUnit.Create(new Guid("00000000-0000-0000-0001-000000000003"), "g",     "Gramm",     "Грамм",   "g",     true),
            MeasurementUnit.Create(new Guid("00000000-0000-0000-0001-000000000004"), "l",     "Litr",      "Литр",    "l",     false),
            MeasurementUnit.Create(new Guid("00000000-0000-0000-0001-000000000005"), "ml",    "Millilitr", "Мл",      "ml",    false),
            MeasurementUnit.Create(new Guid("00000000-0000-0000-0001-000000000006"), "m",     "Metr",      "Метр",    "m",     false),
            MeasurementUnit.Create(new Guid("00000000-0000-0000-0001-000000000007"), "m2",    "Kv. metr",  "Кв.м",    "m²",   false),
            MeasurementUnit.Create(new Guid("00000000-0000-0000-0001-000000000008"), "box",   "Quti",      "Коробка", "box",   false),
            MeasurementUnit.Create(new Guid("00000000-0000-0000-0001-000000000009"), "pack",  "Paket",     "Пачка",   "pack",  false)
        );
    }
}
