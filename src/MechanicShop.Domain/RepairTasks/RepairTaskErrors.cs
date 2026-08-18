using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Domain.RepairTasks
{
    public static class RepairTaskErrors
    {
        public static readonly Error IdRequired =
            Error.Validation("RepairTask.Id.Required", "RepairTask Id is required.");

        public static readonly Error NameRequired =
        Error.Validation("RepairTask.Name.Required", "Name is required.");

        public static readonly Error LaborCostInvalid =
            Error.Validation("RepairTask.LaborCost.Invalid",
                             $"Labor cost must be between {RepairTaskConstant.MinLaborCost} and {RepairTaskConstant.MaxLaborCost:N0}.");

        public static readonly Error DurationInvalid =
            Error.Validation("RepairTask.Duration.Invalid", "Invalid duration selected.");

        public static Error PartsRequired =
       Error.Validation("RepairTask.Parts.Required", "At least one part is required.");

        public readonly static Error DuplicateName =
            Error.Conflict("RepairTaskPart.Duplicate", "A part with the same name already exists in this repair task.");
    }
}
