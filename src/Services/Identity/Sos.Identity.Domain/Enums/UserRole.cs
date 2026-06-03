namespace Sos.Identity.Domain.Entities;

/// <summary>
/// Роли пользователей системы StoreOS
/// </summary>
public enum UserRole
{
    /// <summary>
    /// Супер-администратор — полный доступ ко всей сети магазинов
    /// </summary>
    SuperAdmin = 1,

    /// <summary>
    /// Администратор магазина — управление одним магазином
    /// </summary>
    StoreAdmin = 2,

    /// <summary>
    /// Кассир — только операции на кассе (POS)
    /// </summary>
    Cashier = 3,

    /// <summary>
    /// Кладовщик — управление складом и инвентарём
    /// </summary>
    Warehouseman = 4,

    /// <summary>
    /// Аналитик — просмотр отчётов без права изменений
    /// </summary>
    Analyst = 5
}
