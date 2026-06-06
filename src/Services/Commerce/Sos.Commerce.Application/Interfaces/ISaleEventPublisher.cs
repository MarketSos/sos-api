using Sos.Commerce.Domain.Entities;

namespace Sos.Commerce.Application.Interfaces;

/// <summary>
/// Sotuv hodisalarini nashr qilish interfeysi.
/// Интерфейс публикации событий продажи.
/// </summary>
public interface ISaleEventPublisher
{
    /// <summary>
    /// SaleCompleted hodisasini RabbitMQ ga yuboradi.
    /// Отправляет событие SaleCompleted в RabbitMQ.
    /// </summary>
    Task PublishSaleCompletedAsync(Sale sale, CancellationToken ct = default);
}
