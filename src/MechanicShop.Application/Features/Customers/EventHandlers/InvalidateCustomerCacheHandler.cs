using MechanicShop.Application.Features.Customers.Constants;
using MechanicShop.Domain.Customers.Events;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;

namespace MechanicShop.Application.Features.Customers.EventHandlers
{
    public sealed class InvalidateCustomerCacheHandler(HybridCache cache) :
        INotificationHandler<CustomerCreatedEvent>,
        INotificationHandler<CustomerUpdatedEvent>,
        INotificationHandler<CustomerDeletedEvent>
    {
        private readonly HybridCache _cache = cache;

        public async Task Handle(CustomerCreatedEvent notification, CancellationToken ct)
        {
            await InvalidateAsync(ct);
        }

        public async Task Handle(CustomerUpdatedEvent notification, CancellationToken ct)
        {
            await InvalidateAsync(ct);
        }

        public async Task Handle(CustomerDeletedEvent notification, CancellationToken ct)
        {
            await InvalidateAsync(ct);
        }

        private async Task InvalidateAsync(CancellationToken ct)
        {
            await _cache.RemoveByTagAsync(CustomerCache.Tag, ct);
        }
    }
}
