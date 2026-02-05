using MediatR;

namespace OrderManagement.Domain.Common;

/// <summary>
/// Interface marcadora para eventos de domínio.
/// Herda de INotification do MediatR para facilitar publicação.
/// </summary>
public interface IDomainEvent : INotification
{
    DateTime OccurredAt { get; }
}
