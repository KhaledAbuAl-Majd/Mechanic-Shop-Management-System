using FluentValidation;

namespace MechanicShop.Application.Features.Identity.Commands.RefreshToken
{
    public sealed class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
    {
        public RefreshTokenCommandValidator()
        {
            RuleFor(x => x.RefreshToken).NotEmpty()
                .WithErrorCode("RefreshToken.Required")
                .WithMessage("Refresh Token is required.");

            RuleFor(x => x.ExpiredAccessToken).NotEmpty()
                .WithErrorCode("ExpiredAccessToken.Required")
                .WithMessage("Expired Access Token is required.");
        }
    }
}
