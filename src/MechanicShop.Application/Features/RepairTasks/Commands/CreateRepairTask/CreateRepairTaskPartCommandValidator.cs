using FluentValidation;
using MechanicShop.Domain.RepairTasks.Parts;

namespace MechanicShop.Application.Features.RepairTasks.Commands.CreateRepairTask
{
    public sealed class CreateRepairTaskPartCommandValidator : AbstractValidator<CreateRepairTaskPartCommand>
    {
        public CreateRepairTaskPartCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty()
               .WithErrorCode(PartErrors.NameRequired.Code)
               .WithMessage(PartErrors.NameRequired.Description)
               .MaximumLength(100);

            RuleFor(x => x.Cost).GreaterThan(PartConstant.ExclusiveMinCost).LessThanOrEqualTo(PartConstant.MaxCost)
                .WithErrorCode(PartErrors.CostInvalid.Code)
                .WithMessage(PartErrors.CostInvalid.Description);

            RuleFor(x => x.Quantity).ExclusiveBetween(PartConstant.MinQuantity, PartConstant.MaxQuantity)
                .WithErrorCode(PartErrors.QuantityInvalid.Code)
                .WithMessage(PartErrors.QuantityInvalid.Description);
        }
    }
}
