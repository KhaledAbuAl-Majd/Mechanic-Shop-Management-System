using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.Labors.Dtos;
using MechanicShop.Application.Features.Labors.Mappers;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Identity.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MechanicShop.Application.Features.Labors.Queries.GetLabors
{
    public sealed class GetLaborsQueryHandler(IAppDbContext context) : IRequestHandler<GetLaborsQuery, Result<List<LaborDto>>>
    {
        private readonly IAppDbContext _context = context;

        public async Task<Result<List<LaborDto>>> Handle(GetLaborsQuery query, CancellationToken ct)
        {
            return await _context.Employees.AsNoTracking().Where(e => e.Role == Role.Labor).Select(c => c.ToDto()).ToListAsync(ct);
        }
    }
}
