namespace MechanicShop.Application.Common.Models
{
    public record PaginatedList<T>(
        int PageNumber,
        int PageSize,
        int TotalPages,
        int TotalCount,
        IReadOnlyCollection<T>? Items);
}
