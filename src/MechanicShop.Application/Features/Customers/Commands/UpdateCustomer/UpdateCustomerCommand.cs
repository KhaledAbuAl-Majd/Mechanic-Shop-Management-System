using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.Customers.Constants;
using MechanicShop.Domain.Common.Results;
using MediatR;

namespace MechanicShop.Application.Features.Customers.Commands.UpdateCustomer
{
    public sealed record UpdateCustomerCommand(
        Guid CustomerId,
        string Name,
        string PhoneNumber,
        string Email,
        List<UpdateVehicleCommand> Vehicles

    ) : IRequest<Result<Updated>>, IInvalidateCacheCommand

    {
        public string[] Tags => [CustomerCache.Tag];
    }
}
