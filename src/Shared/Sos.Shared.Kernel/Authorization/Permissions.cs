namespace Sos.Shared.Kernel.Authorization;

/// <summary>
/// Tizim bo'ylab barcha permission konstantlari — modul bo'yicha guruhlangan.
/// JWT tokendagi "permission" claim va RolePermissions da ishlatiladi.
/// </summary>
public static class Permissions
{
    // ── Auth ─────────────────────────────────────────────────────────────────
    public static class Auth
    {
        public const string Me = "auth.me";
    }

    // ── Employees ────────────────────────────────────────────────────────────
    public static class Employees
    {
        public const string Read   = "employees.read";
        public const string Create = "employees.create";
        public const string Update = "employees.update";
        public const string Hire   = "employees.hire";
        public const string Fire   = "employees.fire";
        public const string Delete = "employees.delete";
    }

    // ── Organizations ────────────────────────────────────────────────────────
    public static class Organizations
    {
        public const string Read          = "organizations.read";
        public const string ReadAll       = "organizations.read.all";
        public const string Create        = "organizations.create";
        public const string Update        = "organizations.update";
        public const string Delete        = "organizations.delete";
        public const string ManageMembers = "organizations.manage_members";
    }

    // ── Specializations ───────────────────────────────────────────────────────
    public static class Specializations
    {
        public const string Read   = "specializations.read";
        public const string Create = "specializations.create";
        public const string Update = "specializations.update";
        public const string Delete = "specializations.delete";
    }

    // ── Employee Ranks ────────────────────────────────────────────────────────
    public static class EmployeeRanks
    {
        public const string Read   = "employee_ranks.read";
        public const string Create = "employee_ranks.create";
        public const string Update = "employee_ranks.update";
        public const string Delete = "employee_ranks.delete";
    }

    // ── Products (Catalog) ────────────────────────────────────────────────────
    public static class Products
    {
        public const string Read       = "products.read";
        public const string Create     = "products.create";
        public const string ManageSkus = "products.manage_skus";
    }

    // ── Measurement Units (Catalog) ───────────────────────────────────────────
    public static class MeasurementUnits
    {
        public const string Read   = "measurement_units.read";
        public const string Create = "measurement_units.create";
    }

    // ── Pricing (Catalog) ─────────────────────────────────────────────────────
    public static class Pricing
    {
        public const string Read       = "pricing.read";
        public const string Create     = "pricing.create";
        public const string Deactivate = "pricing.deactivate";
    }

    // ── Stock (Catalog) ───────────────────────────────────────────────────────
    public static class Stock
    {
        public const string Read    = "stock.read";
        public const string ReadLow = "stock.read_low";
        public const string Add     = "stock.add";
        public const string Deduct  = "stock.deduct";
    }

    // ── Sales (Commerce) ──────────────────────────────────────────────────────
    public static class Sales
    {
        public const string Create  = "sales.create";
        public const string AddItem = "sales.add_item";
        public const string Complete = "sales.complete";
        public const string Cancel  = "sales.cancel";
    }

    // ── Customers (Commerce) ─────────────────────────────────────────────────
    public static class Customers
    {
        public const string Read   = "customers.read";
        public const string Search = "customers.search";
        public const string Create = "customers.create";
        public const string Update = "customers.update";
    }

    // ── Loyalty (Commerce) ───────────────────────────────────────────────────
    public static class Loyalty
    {
        public const string Read   = "loyalty.read";
        public const string Create = "loyalty.create";
        public const string Earn   = "loyalty.earn";
        public const string Spend  = "loyalty.spend";
    }

    // ── Analytics ────────────────────────────────────────────────────────────
    public static class Analytics
    {
        public const string SalesSummary   = "analytics.sales_summary";
        public const string RevenueByStore = "analytics.revenue_by_store";
        public const string RecordSale     = "analytics.record_sale";
    }
}
