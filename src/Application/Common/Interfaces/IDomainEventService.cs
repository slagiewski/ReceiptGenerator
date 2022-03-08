using ReceiptGenerator.Domain.Common;

namespace ReceiptGenerator.Application.Common.Interfaces
{
    public interface IDomainEventService
    {
        Task Publish(DomainEvent domainEvent);
    }
}