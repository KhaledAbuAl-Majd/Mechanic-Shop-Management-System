using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.Common.Results.Abstractions;
using MediatR.Pipeline;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Common.Behaviours
{
    public class CacheInvalidationBehaviour<TRequest, TResponse>(ILogger<CacheInvalidationBehaviour<TRequest, TResponse>> logger, HybridCache cache) :
        IRequestPostProcessor<TRequest, TResponse>
        where TRequest : IInvalidateCacheCommand
        where TResponse : IResult

    {
        private readonly ILogger<CacheInvalidationBehaviour<TRequest, TResponse>> _logger = logger;
        private readonly HybridCache _cache = cache;

        public async Task Process(TRequest request, TResponse response, CancellationToken ct)
        {
            if (!response.IsSuccess || request.Tags is null || request.Tags.Length == 0)
            {
                return;
            }

            foreach (var tag in request.Tags)
                await _cache.RemoveByTagAsync(tag, ct);

            _logger.LogInformation("Cache invalidated for tags: {@Tags}", request.Tags);
        }
    }
}
