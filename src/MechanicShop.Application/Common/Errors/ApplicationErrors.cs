using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Application.Common.Errors
{
    public static class ApplicationErrors
    {
        public static Error CustomerNotFound =>
            Error.NotFound("ApplicationError.Customer.NotFound", "Customer does not exists.");

        public static Error VehicleNotFound =>
            Error.NotFound("ApplicationErrors.Vehicle.NotFound", "Vehicle does not exist.");

        public static Error RepairTaskNotFound =>
            Error.NotFound(
                    "ApplicationErrors.RepairTask.NotFound",
                    "Repair task does not exist.");

    }
}
