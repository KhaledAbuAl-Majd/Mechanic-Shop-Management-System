using MediatR;

namespace MechanicShop.Application.Common.Interfaces
{
    public interface ICachedQuery
    {
        string Key { get; }
        string[] Tags { get; }
        TimeSpan Expiration { get; }
    }
    public interface ICachedQuery<TResponse> : IRequest<TResponse>, ICachedQuery;
}
