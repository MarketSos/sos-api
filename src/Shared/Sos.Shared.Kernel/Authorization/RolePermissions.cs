namespace Sos.Shared.Kernel.Authorization;

/// <summary>
/// Har bir rolga tegishli permission ro'yxati.
/// Ierarxik: SuperAdmin ⊇ StoreAdmin ⊇ Cashier.
/// </summary>
public static class RolePermissions
{
    // ── Cashier ───────────────────────────────────────────────────────────────
    // Faqat POS operatsiyalari: sotuv, mijozlar, loyallik, mahsulot o'qish
    public static readonly IReadOnlyList<string> Cashier =
    [
        Permissions.Auth.Me,

        Permissions.Products.Read,

        Permissions.Stock.Read,
        Permissions.Stock.Deduct,

        Permissions.Sales.Create,
        Permissions.Sales.AddItem,
        Permissions.Sales.Complete,
        Permissions.Sales.Cancel,

        Permissions.Customers.Read,
        Permissions.Customers.Search,
        Permissions.Customers.Create,
        Permissions.Customers.Update,

        Permissions.Loyalty.Read,
        Permissions.Loyalty.Create,
        Permissions.Loyalty.Earn,
        Permissions.Loyalty.Spend,

        Permissions.Analytics.RecordSale,
    ];

    // ── StoreAdmin ────────────────────────────────────────────────────────────
    // Do'kon boshqaruvi: Cashier + xodimlar, katalog, ombor, narxlar
    public static readonly IReadOnlyList<string> StoreAdmin =
    [
        ..Cashier,

        Permissions.Employees.Read,
        Permissions.Employees.Create,
        Permissions.Employees.Update,
        Permissions.Employees.Hire,
        Permissions.Employees.Fire,
        Permissions.Employees.Delete,

        Permissions.Organizations.Read,
        Permissions.Organizations.Update,
        Permissions.Organizations.ManageMembers,

        Permissions.Specializations.Read,
        Permissions.Specializations.Create,
        Permissions.Specializations.Update,
        Permissions.Specializations.Delete,

        Permissions.EmployeeRanks.Read,
        Permissions.EmployeeRanks.Create,
        Permissions.EmployeeRanks.Update,
        Permissions.EmployeeRanks.Delete,

        Permissions.Products.Create,
        Permissions.Products.ManageSkus,

        Permissions.MeasurementUnits.Read,

        Permissions.Pricing.Read,
        Permissions.Pricing.Create,
        Permissions.Pricing.Deactivate,

        Permissions.Stock.ReadLow,
        Permissions.Stock.Add,

        Permissions.Analytics.SalesSummary,
    ];

    // ── SuperAdmin ────────────────────────────────────────────────────────────
    // To'liq kirish: StoreAdmin + global tashkilot va analytics
    public static readonly IReadOnlyList<string> SuperAdmin =
    [
        ..StoreAdmin,

        Permissions.Organizations.ReadAll,
        Permissions.Organizations.Create,
        Permissions.Organizations.Delete,

        Permissions.MeasurementUnits.Create,

        Permissions.Analytics.RevenueByStore,
    ];
}
