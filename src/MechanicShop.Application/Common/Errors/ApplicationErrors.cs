using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Application.Common.Errors
{
    public static class ApplicationErrors
    {
        public static Error WorkOrderOutsideOperatingHour(DateTimeOffset startAtUtc, DateTimeOffset endAtUtc) =>
            Error.Conflict(
           "ApplicationErrors.WorkOrder.Outside.OperatingHours",
           $"The WorkOrder time ({startAtUtc} ? {endAtUtc}) is outside of store operating hours.");

        public static Error WorkOrderNotFound => Error.NotFound(
       "ApplicationErrors.WorkOrder.NotFound",
       "WorkOrder does not exist.");

        public static Error LaborOccupied =>
        Error.Conflict(
               "Employee.LaborOccupied",
               "Labor is already occupied during the requested time.");

        public static Error CustomerNotFound =>
            Error.NotFound("ApplicationError.Customer.NotFound", "Customer does not exists.");

        public static Error VehicleNotFound =>
            Error.NotFound("ApplicationErrors.Vehicle.NotFound", "Vehicle does not exist.");

        public static Error RepairTaskNotFound =>
            Error.NotFound(
                    "ApplicationErrors.RepairTask.NotFound",
                    "Repair task does not exist.");

        public static Error LaborNotFound =>
            Error.NotFound("Employee.LaborNotFound", "Labor does not exist.");

    }
}
