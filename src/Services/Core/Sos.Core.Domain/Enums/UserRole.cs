namespace Sos.Core.Domain.Enums;

/// <summary>
/// Foydalanuvchi rollari.
/// Роли пользователей.
/// </summary>
public enum UserRoles
{
    SuperAdmin   = 1,  // To'liq kirish / Полный доступ
    StoreAdmin   = 2,  // Do'kon boshqaruvi / Управление магазином
    Cashier      = 3,  // Kassa / Касса
    Warehouseman = 4,  // Ombor / Склад
    Analyst      = 5   // Hisobotlar / Отчёты
}
