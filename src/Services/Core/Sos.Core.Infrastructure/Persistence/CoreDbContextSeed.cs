using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sos.Core.Domain.Entities;
using Sos.Core.Domain.Entities.Identity;
using Sos.Core.Domain.Enums;
using Sos.Shared.Kernel.Authorization;

namespace Sos.Core.Infrastructure.Persistence;

public static class CoreDbContextSeed
{
    // ── Doimiy ID'lar (idempotent seed uchun) ─────────────────────────────────
    private static readonly Guid OrgId  = new("10000000-0000-0000-0000-000000000001");
    private static readonly Guid UserId = new("10000000-0000-0000-0000-000000000002");
    private static readonly Guid EmpId  = new("10000000-0000-0000-0000-000000000003");

    // ── OrgType ID'lari ────────────────────────────────────────────────────────
    private static readonly Guid OrgTypeGroceryId    = new("50000000-0000-0000-0000-000000000001");
    private static readonly Guid OrgTypePharmacyId   = new("50000000-0000-0000-0000-000000000002");
    private static readonly Guid OrgTypeElectronicsId= new("50000000-0000-0000-0000-000000000003");
    private static readonly Guid OrgTypeClothingId   = new("50000000-0000-0000-0000-000000000004");
    private static readonly Guid OrgTypeBuildingId   = new("50000000-0000-0000-0000-000000000005");
    private static readonly Guid OrgTypeSportsId     = new("50000000-0000-0000-0000-000000000006");
    private static readonly Guid OrgTypeBookId       = new("50000000-0000-0000-0000-000000000007");
    private static readonly Guid OrgTypeHouseholdId  = new("50000000-0000-0000-0000-000000000008");
    private static readonly Guid OrgTypeCafeId       = new("50000000-0000-0000-0000-000000000009");
    private static readonly Guid OrgTypeGeneralId    = new("50000000-0000-0000-0000-000000000010");

    // ── EmployeeRank ID'lari ───────────────────────────────────────────────────
    private static readonly Guid RankDirectorId   = new("20000000-0000-0000-0000-000000000001");
    private static readonly Guid RankManagerId    = new("20000000-0000-0000-0000-000000000002");
    private static readonly Guid RankSeniorId     = new("20000000-0000-0000-0000-000000000003");
    private static readonly Guid RankSpecialistId = new("20000000-0000-0000-0000-000000000004");
    private static readonly Guid RankJuniorId     = new("20000000-0000-0000-0000-000000000005");

    // ── Specialization ID'lari ────────────────────────────────────────────────
    private static readonly Guid SpecCashierId      = new("30000000-0000-0000-0000-000000000001");
    private static readonly Guid SpecSalespersonId  = new("30000000-0000-0000-0000-000000000002");
    private static readonly Guid SpecManagerId      = new("30000000-0000-0000-0000-000000000003");
    private static readonly Guid SpecWarehousemanId = new("30000000-0000-0000-0000-000000000004");
    private static readonly Guid SpecAccountantId   = new("30000000-0000-0000-0000-000000000005");
    private static readonly Guid SpecSecurityId     = new("30000000-0000-0000-0000-000000000006");
    private static readonly Guid SpecDriverId       = new("30000000-0000-0000-0000-000000000007");
    private static readonly Guid SpecCleanerId      = new("30000000-0000-0000-0000-000000000008");

    // ── Role ID'lari (doimiy) ─────────────────────────────────────────────────
    private static readonly Guid RoleIdSuperAdmin = new("40000000-0000-0000-0000-000000000001");
    private static readonly Guid RoleIdStoreAdmin = new("40000000-0000-0000-0000-000000000002");
    private static readonly Guid RoleIdCashier    = new("40000000-0000-0000-0000-000000000003");

    public static async Task SeedAsync(
        CoreDbContext     db,
        UserManager<User> userManager,
        RoleManager<Role> roleManager,
        ILogger           logger)
    {
        await SeedOrgTypesAsync(db, logger);
        await SeedEmployeeRanksAsync(db, logger);
        await SeedSpecializationsAsync(db, logger);
        await SeedOrganizationAsync(db, logger);
        await SeedRolesAsync(roleManager, logger);
        await SeedSuperAdminAsync(db, userManager, logger);
    }

    // ── OrgType (Biznes turlari) ──────────────────────────────────────────────
    private static async Task SeedOrgTypesAsync(CoreDbContext db, ILogger logger)
    {
        if (await db.OrganizationTypes.AnyAsync()) return;

        var types = new[]
        {
            OrganizationType.Create(OrgTypeGroceryId,    "GROCERY",     "Oziq-ovqat do'koni",    "Продуктовый магазин",    nameEn: "Grocery Store",      nameUzCyrl: "Озиқ-овқат дўкони",     icon: "pi-shopping-basket"),
            OrganizationType.Create(OrgTypePharmacyId,   "PHARMACY",    "Apteka",                "Аптека",                 nameEn: "Pharmacy",            nameUzCyrl: "Аптека",                 icon: "pi-heart"),
            OrganizationType.Create(OrgTypeElectronicsId,"ELECTRONICS", "Elektronika do'koni",   "Магазин электроники",    nameEn: "Electronics Store",   nameUzCyrl: "Электроника дўкони",     icon: "pi-tablet"),
            OrganizationType.Create(OrgTypeClothingId,   "CLOTHING",    "Kiyim-kechak do'koni",  "Магазин одежды",         nameEn: "Clothing Store",      nameUzCyrl: "Кийим-кечак дўкони",    icon: "pi-tag"),
            OrganizationType.Create(OrgTypeBuildingId,   "BUILDING",    "Qurilish materiallari", "Стройматериалы",         nameEn: "Building Materials",  nameUzCyrl: "Қурилиш материаллари",  icon: "pi-hammer"),
            OrganizationType.Create(OrgTypeSportsId,     "SPORTS",      "Sport do'koni",         "Спортивный магазин",     nameEn: "Sports Store",        nameUzCyrl: "Спорт дўкони",          icon: "pi-sparkles"),
            OrganizationType.Create(OrgTypeBookId,       "BOOK",        "Kitob do'koni",         "Книжный магазин",        nameEn: "Book Store",          nameUzCyrl: "Китоб дўкони",          icon: "pi-book"),
            OrganizationType.Create(OrgTypeHouseholdId,  "HOUSEHOLD",   "Uy-ro'zg'or buyumlari", "Товары для дома",        nameEn: "Household Goods",     nameUzCyrl: "Уй-рўзғор буюмлари",   icon: "pi-home"),
            OrganizationType.Create(OrgTypeCafeId,       "CAFE",        "Kafe / Restoran",       "Кафе / Ресторан",        nameEn: "Cafe / Restaurant",   nameUzCyrl: "Кафе / Ресторан",       icon: "pi-star"),
            OrganizationType.Create(OrgTypeGeneralId,    "GENERAL",     "Umumiy do'kon",         "Общий магазин",          nameEn: "General Store",       nameUzCyrl: "Умумий дўкон",          icon: "pi-shop"),
        };

        await db.OrganizationTypes.AddRangeAsync(types);
        await db.SaveChangesAsync();
        logger.LogInformation("OrgTypes seed: {Count} ta yozuv qo'shildi.", types.Length);
    }

    // ── EmployeeRank ──────────────────────────────────────────────────────────

    private static async Task SeedEmployeeRanksAsync(CoreDbContext db, ILogger logger)
    {
        if (await db.EmployeeRanks.AnyAsync()) return;

        var ranks = new[]
        {
            EmployeeRank.Create(RankDirectorId,   "DIRECTOR",
                nameUz: "Direktor",         nameRu: "Директор",           nameEn: "Director",          nameUzCyrl: "Директор"),
            EmployeeRank.Create(RankManagerId,    "MANAGER",
                nameUz: "Menejer",          nameRu: "Менеджер",           nameEn: "Manager",           nameUzCyrl: "Менежер"),
            EmployeeRank.Create(RankSeniorId,     "SENIOR",
                nameUz: "Katta mutaxassis", nameRu: "Старший специалист", nameEn: "Senior Specialist", nameUzCyrl: "Катта мутахассис"),
            EmployeeRank.Create(RankSpecialistId, "SPECIALIST",
                nameUz: "Mutaxassis",       nameRu: "Специалист",         nameEn: "Specialist",        nameUzCyrl: "Мутахассис"),
            EmployeeRank.Create(RankJuniorId,     "JUNIOR",
                nameUz: "Yordamchi",        nameRu: "Помощник",           nameEn: "Junior",            nameUzCyrl: "Ёрдамчи"),
        };

        foreach (var r in ranks) r.OrganizationId = OrgId;

        await db.EmployeeRanks.AddRangeAsync(ranks);
        await db.SaveChangesAsync();
        logger.LogInformation("EmployeeRanks seed: {Count} ta yozuv qo'shildi.", ranks.Length);
    }

    // ── Specialization ────────────────────────────────────────────────────────

    private static async Task SeedSpecializationsAsync(CoreDbContext db, ILogger logger)
    {
        if (await db.Specializations.AnyAsync()) return;

        var specs = new[]
        {
            Specialization.Create(SpecCashierId,      "CASHIER",
                nameUz: "Kassir",            nameRu: "Кассир",               nameEn: "Cashier",        nameUzCyrl: "Кассир"),
            Specialization.Create(SpecSalespersonId,  "SALESPERSON",
                nameUz: "Sotuvchi",          nameRu: "Продавец",             nameEn: "Salesperson",    nameUzCyrl: "Сотувчи"),
            Specialization.Create(SpecManagerId,      "MANAGER",
                nameUz: "Savdo menejeri",    nameRu: "Менеджер по продажам", nameEn: "Sales Manager",  nameUzCyrl: "Савдо менежери"),
            Specialization.Create(SpecWarehousemanId, "WAREHOUSEMAN",
                nameUz: "Omborchi",          nameRu: "Кладовщик",            nameEn: "Warehouseman",   nameUzCyrl: "Омборчи"),
            Specialization.Create(SpecAccountantId,   "ACCOUNTANT",
                nameUz: "Hisobchi",          nameRu: "Бухгалтер",            nameEn: "Accountant",     nameUzCyrl: "Ҳисобчи"),
            Specialization.Create(SpecSecurityId,     "SECURITY",
                nameUz: "Xavfsizlik xodimi", nameRu: "Сотрудник охраны",    nameEn: "Security Guard", nameUzCyrl: "Хавфсизлик ходими"),
            Specialization.Create(SpecDriverId,       "DRIVER",
                nameUz: "Haydovchi",         nameRu: "Водитель",             nameEn: "Driver",         nameUzCyrl: "Ҳайдовчи"),
            Specialization.Create(SpecCleanerId,      "CLEANER",
                nameUz: "Farrosh",           nameRu: "Уборщик",              nameEn: "Cleaner",        nameUzCyrl: "Фаррош"),
        };

        foreach (var s in specs) s.OrganizationId = OrgId;

        await db.Specializations.AddRangeAsync(specs);
        await db.SaveChangesAsync();
        logger.LogInformation("Specializations seed: {Count} ta yozuv qo'shildi.", specs.Length);
    }

    // ── Organization ──────────────────────────────────────────────────────────

    private static async Task SeedOrganizationAsync(CoreDbContext db, ILogger logger)
    {
        if (await db.Organizations.AnyAsync(o => o.Id == OrgId)) return;

        var address = Address.Create(
            addressLine:    "Toshkent shahri, Yunusobod tumani",
            region:         "Toshkent shahri",
            district:       "Yunusobod tumani",
            mahalla:        "Yunusobod MFY",
            street:         "Amir Temur ko'chasi",
            referencePoint: "Toshkent metro yaqinida");

        await db.Addresses.AddAsync(address);
        await db.SaveChangesAsync();

        var org = Organization.Create(
            id:          OrgId,
            nameUz:      "Asosiy tashkilot",
            nameRu:      "Главная организация",
            slug:        "main",
            ownerUserId: UserId,
            nameEn:      "Main Organization",
            nameUzCyrl:  "Асосий ташкилот");

        org.Code      = "MAIN-001";
        org.OrgTypeId = OrgTypeGroceryId;
        org.Address   = address;
        org.Classify(OwnershipType.System, OrganizationLevel.Root);

        await db.Organizations.AddAsync(org);
        await db.SaveChangesAsync();
        logger.LogInformation("Organization seed: 'Main' tashkilot qo'shildi.");
    }

    // ── Identity Roles + Permission Claims ────────────────────────────────────
    // Har bir rolga tegishli permissionlar Permissions.cs dan olinadi.
    // SuperAdmin — barcha permissionlar (to'liq kirish).

    private static async Task SeedRolesAsync(RoleManager<Role> roleManager, ILogger logger)
    {
        var roles = new[]
        {
            (
                Id:          RoleIdSuperAdmin,
                Name:        UserRoles.SuperAdmin.ToString(),
                NameUz:      "Super administrator",
                NameUzCyrl:  "Супер администратор",
                NameRu:      "Супер администратор",
                NameEn:      (string?)"Super Admin",
                NameKk:      (string?)null,
                Permissions: RolePermissions.SuperAdmin
            ),
            (
                Id:          RoleIdStoreAdmin,
                Name:        UserRoles.StoreAdmin.ToString(),
                NameUz:      "Do'kon administratori",
                NameUzCyrl:  "Дўкон администратори",
                NameRu:      "Администратор магазина",
                NameEn:      (string?)"Store Admin",
                NameKk:      (string?)null,
                Permissions: RolePermissions.StoreAdmin
            ),
            (
                Id:          RoleIdCashier,
                Name:        UserRoles.Cashier.ToString(),
                NameUz:      "Kassir",
                NameUzCyrl:  "Кассир",
                NameRu:      "Кассир",
                NameEn:      (string?)"Cashier",
                NameKk:      (string?)null,
                Permissions: RolePermissions.Cashier
            ),
        };

        foreach (var (id, name, nameUz, nameUzCyrl, nameRu, nameEn, nameKk, permissions) in roles)
        {
            var existing = await roleManager.FindByNameAsync(name);
            if (existing is not null) continue;

            var role = new Role
            {
                Id        = id,
                Name      = name,
                NameUz    = nameUz,
                NameUzCyrl = nameUzCyrl,
                NameRu    = nameRu,
                NameEn    = nameEn,
                NameKk    = nameKk,
            };
            var result = await roleManager.CreateAsync(role);
            if (!result.Succeeded)
            {
                logger.LogError("'{Role}' roli yaratishda xato: {Errors}",
                    name, string.Join(", ", result.Errors.Select(e => e.Description)));
                continue;
            }

            foreach (var permission in permissions)
                await roleManager.AddClaimAsync(role, new Claim("permission", permission));

            logger.LogInformation("'{Role}' roli seed qilindi ({Count} ta permission).",
                name, permissions.Count);
        }
    }

    // ── SuperAdmin User + Employee ─────────────────────────────────────────────

    private static async Task SeedSuperAdminAsync(
        CoreDbContext     db,
        UserManager<User> userManager,
        ILogger           logger)
    {
        if (await db.Users.AnyAsync(u => u.Id == UserId)) return;

        var user = User.Create(
            id:             UserId,
            userName:       "superadmin",
            email:          "admin@sos.uz",
            organizationId: OrgId);

        var result = await userManager.CreateAsync(user, "Admin@123456");
        if (!result.Succeeded)
        {
            logger.LogError("SuperAdmin yaratishda xato: {Errors}",
                string.Join(", ", result.Errors.Select(e => e.Description)));
            return;
        }

        await userManager.AddToRoleAsync(user, UserRoles.SuperAdmin.ToString());

        var employee = Employee.Create(
            userId:           UserId,
            firstName:        "Super",
            lastName:         "Admin",
            specializationId: SpecManagerId,
            employeeRankId:   RankDirectorId,
            hireDate:         DateOnly.FromDateTime(DateTime.UtcNow));

        employee.Id             = EmpId;
        employee.OrganizationId = OrgId;

        await db.Employees.AddAsync(employee);
        await db.SaveChangesAsync();

        logger.LogInformation("SuperAdmin seed qo'shildi. Email: admin@sos.uz");
    }
}
