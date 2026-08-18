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

            RuleFor(x => x.Cost).GreaterThan(0)
                .WithMessage("Part cost must be greater than 0.");

            RuleFor(x => x.Quantity).GreaterThan(0)
                .WithMessage("Quantity must be at least 1.");
        }
    }
}
