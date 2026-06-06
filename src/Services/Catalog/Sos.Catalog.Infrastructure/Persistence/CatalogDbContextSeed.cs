using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sos.Catalog.Domain.Entities;

namespace Sos.Catalog.Infrastructure.Persistence;

/// <summary>
/// Katalog uchun boshlang'ich ma'lumotlar.
/// Начальные данные для каталога.
/// </summary>
public static class CatalogDbContextSeed
{
    // ── O'lchov birliklari ID'lari ────────────────────────────────────────────
    public static readonly Guid UnitDonaId  = Guid.NewGuid();
    public static readonly Guid UnitKgId    = Guid.NewGuid();
    public static readonly Guid UnitGId     = Guid.NewGuid();
    public static readonly Guid UnitLId     = Guid.NewGuid();
    public static readonly Guid UnitMlId    = Guid.NewGuid();
    public static readonly Guid UnitMetrId  = Guid.NewGuid();
    public static readonly Guid UnitQutiId  = Guid.NewGuid();
    public static readonly Guid UnitPaketId = Guid.NewGuid();
    public static readonly Guid UnitJuftId  = Guid.NewGuid();

    // ── Kategoriya ID'lari (root) ──────────────────────────────────────────────
    public static readonly Guid CatFoodId        = Guid.NewGuid();
    public static readonly Guid CatHouseholdId   = Guid.NewGuid();
    public static readonly Guid CatElectronicsId = Guid.NewGuid();
    public static readonly Guid CatClothingId    = Guid.NewGuid();
    public static readonly Guid CatStationaryId  = Guid.NewGuid();

    // ── Kategoriya ID'lari (Food ichida) ──────────────────────────────────────
    public static readonly Guid CatBakeryId       = Guid.NewGuid();
    public static readonly Guid CatMeatId         = Guid.NewGuid();
    public static readonly Guid CatDairyId        = Guid.NewGuid();
    public static readonly Guid CatFruitsVegsId   = Guid.NewGuid();
    public static readonly Guid CatDrinksId       = Guid.NewGuid();
    public static readonly Guid CatSweetsId       = Guid.NewGuid();
    public static readonly Guid CatCerealsId      = Guid.NewGuid();
    public static readonly Guid CatCannedId       = Guid.NewGuid();
    public static readonly Guid CatOilsId         = Guid.NewGuid();

    // ── Kategoriya ID'lari (Household ichida) ────────────────────────────────
    public static readonly Guid CatCleaningId = Guid.NewGuid();
    public static readonly Guid CatHygieneId  = Guid.NewGuid();

    // ── Mahsulot ID'lari ──────────────────────────────────────────────────────
    private static readonly Guid ProdNonId        = Guid.NewGuid();
    private static readonly Guid ProdSutId        = Guid.NewGuid();
    private static readonly Guid ProdQatiqId      = Guid.NewGuid();
    private static readonly Guid ProdTuxumId      = Guid.NewGuid();
    private static readonly Guid ProdShakarId     = Guid.NewGuid();
    private static readonly Guid ProdUnId         = Guid.NewGuid();
    private static readonly Guid ProdGuruchId     = Guid.NewGuid();
    private static readonly Guid ProdYogId        = Guid.NewGuid();
    private static readonly Guid ProdChoyId       = Guid.NewGuid();
    private static readonly Guid ProdKofId        = Guid.NewGuid();
    private static readonly Guid ProdKolbasaId    = Guid.NewGuid();
    private static readonly Guid ProdTavukId      = Guid.NewGuid();
    private static readonly Guid ProdBananaId     = Guid.NewGuid();
    private static readonly Guid ProdOlmaId       = Guid.NewGuid();
    private static readonly Guid ProdShampunId    = Guid.NewGuid();
    private static readonly Guid ProdTishkremId   = Guid.NewGuid();
    private static readonly Guid ProdKirYuvishId  = Guid.NewGuid();
    private static readonly Guid ProdIdishYuvishId = Guid.NewGuid();
    private static readonly Guid ProdShocoladId   = Guid.NewGuid();
    private static readonly Guid ProdPechenyeId   = Guid.NewGuid();

    public static async Task SeedAsync(CatalogDbContext db, ILogger logger)
    {
        await SeedMeasurementUnitsAsync(db, logger);
        await SeedCategoriesAsync(db, logger);
        await SeedProductsAsync(db, logger);
    }

    private static async Task SeedMeasurementUnitsAsync(CatalogDbContext db, ILogger logger)
    {
        if (await db.MeasurementUnits.AnyAsync()) return;

        var units = new[]
        {
            MeasurementUnit.Create(UnitDonaId,  "dona",  nameUz: "Dona",   nameRu: "Штука",   nameEn: "Piece"),
            MeasurementUnit.Create(UnitKgId,    "kg",    nameUz: "Kilogram", nameRu: "Килограмм", nameEn: "Kilogram"),
            MeasurementUnit.Create(UnitGId,     "g",     nameUz: "Gram",   nameRu: "Грамм",   nameEn: "Gram"),
            MeasurementUnit.Create(UnitLId,     "l",     nameUz: "Litr",   nameRu: "Литр",    nameEn: "Litre"),
            MeasurementUnit.Create(UnitMlId,    "ml",    nameUz: "Millilitr", nameRu: "Миллилитр", nameEn: "Millilitre"),
            MeasurementUnit.Create(UnitMetrId,  "m",     nameUz: "Metr",   nameRu: "Метр",    nameEn: "Metre"),
            MeasurementUnit.Create(UnitQutiId,  "quti",  nameUz: "Quti",   nameRu: "Коробка", nameEn: "Box"),
            MeasurementUnit.Create(UnitPaketId, "paket", nameUz: "Paket",  nameRu: "Пакет",   nameEn: "Packet"),
            MeasurementUnit.Create(UnitJuftId,  "juft",  nameUz: "Juft",   nameRu: "Пара",    nameEn: "Pair"),
        };

        await db.MeasurementUnits.AddRangeAsync(units);
        await db.SaveChangesAsync();
        logger.LogInformation("MeasurementUnits seed: {Count} ta yozuv qo'shildi.", units.Length);
    }

    private static async Task SeedCategoriesAsync(CatalogDbContext db, ILogger logger)
    {
        if (await db.Categories.AnyAsync()) return;

        // Root kategoriyalar
        var roots = new[]
        {
            Category.Create(CatFoodId,        nameUz: "Oziq-ovqat",         nameRu: "Продукты питания",   nameEn: "Food"),
            Category.Create(CatHouseholdId,   nameUz: "Maishiy tovarlar",   nameRu: "Товары для дома",    nameEn: "Household"),
            Category.Create(CatElectronicsId, nameUz: "Elektronika",        nameRu: "Электроника",        nameEn: "Electronics"),
            Category.Create(CatClothingId,    nameUz: "Kiyim-kechak",      nameRu: "Одежда",             nameEn: "Clothing"),
            Category.Create(CatStationaryId,  nameUz: "Maktab jihozlari",  nameRu: "Канцтовары",         nameEn: "Stationary"),
        };

        await db.Categories.AddRangeAsync(roots);
        await db.SaveChangesAsync();

        // Oziq-ovqat ichidagi kichik kategoriyalar
        var foodSubs = new[]
        {
            Category.Create(CatBakeryId,     nameUz: "Non mahsulotlari",   nameRu: "Хлебобулочные",      nameEn: "Bakery",           parentId: CatFoodId),
            Category.Create(CatMeatId,       nameUz: "Go'sht mahsulotlari",nameRu: "Мясные продукты",    nameEn: "Meat",             parentId: CatFoodId),
            Category.Create(CatDairyId,      nameUz: "Sut mahsulotlari",   nameRu: "Молочные продукты",  nameEn: "Dairy",            parentId: CatFoodId),
            Category.Create(CatFruitsVegsId, nameUz: "Sabzavot va mevalar",nameRu: "Овощи и фрукты",     nameEn: "Fruits & Vegetables", parentId: CatFoodId),
            Category.Create(CatDrinksId,     nameUz: "Ichimliklar",        nameRu: "Напитки",            nameEn: "Drinks",           parentId: CatFoodId),
            Category.Create(CatSweetsId,     nameUz: "Shirinliklar",       nameRu: "Сладости",           nameEn: "Sweets",           parentId: CatFoodId),
            Category.Create(CatCerealsId,    nameUz: "Don mahsulotlari",   nameRu: "Крупы и зерновые",   nameEn: "Cereals",          parentId: CatFoodId),
            Category.Create(CatCannedId,     nameUz: "Konservalar",        nameRu: "Консервы",           nameEn: "Canned Goods",     parentId: CatFoodId),
            Category.Create(CatOilsId,       nameUz: "Yog' va sous",       nameRu: "Масла и соусы",      nameEn: "Oils & Sauces",    parentId: CatFoodId),
        };

        // Maishiy tovarlar ichidagi kichik kategoriyalar
        var householdSubs = new[]
        {
            Category.Create(CatCleaningId, nameUz: "Tozalash vositalari", nameRu: "Чистящие средства", nameEn: "Cleaning", parentId: CatHouseholdId),
            Category.Create(CatHygieneId,  nameUz: "Shaxsiy gigiyena",   nameRu: "Личная гигиена",    nameEn: "Hygiene",  parentId: CatHouseholdId),
        };

        await db.Categories.AddRangeAsync(foodSubs.Concat(householdSubs));
        await db.SaveChangesAsync();
        logger.LogInformation("Categories seed: {Count} ta yozuv qo'shildi.", roots.Length + foodSubs.Length + householdSubs.Length);
    }

    // ── Mahsulotlar ───────────────────────────────────────────────────────────

    private static async Task SeedProductsAsync(CatalogDbContext db, ILogger logger)
    {
        if (await db.Products.AnyAsync()) return;

        var products = new[]
        {
            // ── Non mahsulotlari ──────────────────────────────────────────────
            Product.Create(ProdNonId,   nameUz: "Oq non",         nameRu: "Белый хлеб",       barcode: "4600001000001", categoryId: CatBakeryId),

            // ── Sut mahsulotlari ──────────────────────────────────────────────
            Product.Create(ProdSutId,   nameUz: "Sut 1L",         nameRu: "Молоко 1Л",        barcode: "4600001000002", categoryId: CatDairyId),
            Product.Create(ProdQatiqId, nameUz: "Qatiq 500g",     nameRu: "Кефир 500г",       barcode: "4600001000003", categoryId: CatDairyId),
            Product.Create(ProdTuxumId, nameUz: "Tuxum 10 dona",  nameRu: "Яйца 10 шт",       barcode: "4600001000004", categoryId: CatDairyId),

            // ── Don mahsulotlari ──────────────────────────────────────────────
            Product.Create(ProdShakarId, nameUz: "Shakar 1kg",   nameRu: "Сахар 1кг",         barcode: "4600001000005", categoryId: CatCerealsId),
            Product.Create(ProdUnId,     nameUz: "Un 2kg",        nameRu: "Мука 2кг",          barcode: "4600001000006", categoryId: CatCerealsId),
            Product.Create(ProdGuruchId, nameUz: "Guruch 1kg",    nameRu: "Рис 1кг",           barcode: "4600001000007", categoryId: CatCerealsId),

            // ── Yog' va sous ──────────────────────────────────────────────────
            Product.Create(ProdYogId,   nameUz: "O'simlik yog'i 1L", nameRu: "Растительное масло 1Л", barcode: "4600001000008", categoryId: CatOilsId),

            // ── Ichimliklar ───────────────────────────────────────────────────
            Product.Create(ProdChoyId,  nameUz: "Qora choy 100g",   nameRu: "Чёрный чай 100г",   barcode: "4600001000009", categoryId: CatDrinksId),
            Product.Create(ProdKofId,   nameUz: "Kofe 200g",         nameRu: "Кофе 200г",         barcode: "4600001000010", categoryId: CatDrinksId),

            // ── Go'sht ────────────────────────────────────────────────────────
            Product.Create(ProdKolbasaId, nameUz: "Kolbasa 500g",   nameRu: "Колбаса 500г",      barcode: "4600001000011", categoryId: CatMeatId),
            Product.Create(ProdTavukId,   nameUz: "Tovuq go'shti 1kg", nameRu: "Куриное мясо 1кг", barcode: "4600001000012", categoryId: CatMeatId),

            // ── Sabzavot va mevalar ───────────────────────────────────────────
            Product.Create(ProdBananaId, nameUz: "Banan 1kg",      nameRu: "Бананы 1кг",        barcode: "4600001000013", categoryId: CatFruitsVegsId),
            Product.Create(ProdOlmaId,   nameUz: "Olma 1kg",       nameRu: "Яблоки 1кг",        barcode: "4600001000014", categoryId: CatFruitsVegsId),

            // ── Shaxsiy gigiyena ──────────────────────────────────────────────
            Product.Create(ProdShampunId,  nameUz: "Shampun 400ml",   nameRu: "Шампунь 400мл",   barcode: "4600001000015", categoryId: CatHygieneId),
            Product.Create(ProdTishkremId, nameUz: "Tish kremi 100g", nameRu: "Зубная паста 100г", barcode: "4600001000016", categoryId: CatHygieneId),

            // ── Tozalash vositalari ───────────────────────────────────────────
            Product.Create(ProdKirYuvishId,   nameUz: "Kir yuvish kukuni 1kg", nameRu: "Стиральный порошок 1кг", barcode: "4600001000017", categoryId: CatCleaningId),
            Product.Create(ProdIdishYuvishId, nameUz: "Idish yuvish vositasi 500ml", nameRu: "Средство для посуды 500мл", barcode: "4600001000018", categoryId: CatCleaningId),

            // ── Shirinliklar ──────────────────────────────────────────────────
            Product.Create(ProdShocoladId, nameUz: "Shokolad 100g",  nameRu: "Шоколад 100г",    barcode: "4600001000019", categoryId: CatSweetsId),
            Product.Create(ProdPechenyeId, nameUz: "Pechenye 200g",  nameRu: "Печенье 200г",    barcode: "4600001000020", categoryId: CatSweetsId),
        };

        await db.Products.AddRangeAsync(products);
        await db.SaveChangesAsync();
        logger.LogInformation("Products seed: {Count} ta mahsulot qo'shildi.", products.Length);
    }
}
