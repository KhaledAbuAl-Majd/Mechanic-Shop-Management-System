using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.Identity.Dtos;
using MechanicShop.Application.Features.Identity.Interfaces;
using MechanicShop.Domain.Common.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.Identity.Commands.GenerateToken
{
    public sealed class GenerateTokenCommandHandler(
        ILogger<GenerateTokenCommandHandler> logger,
        IIdentityService identityService,
        ITokenProvider tokenProvider) : IRequestHandler<GenerateTokenCommand, Result<TokenDto>>
    {
        private readonly ILogger<GenerateTokenCommandHandler> _logger = logger;
        private readonly IIdentityService _identityService = identityService;
        private readonly ITokenProvider _tokenProvider = tokenProvider;

        public async Task<Result<TokenDto>> Handle(GenerateTokenCommand command, CancellationToken ct)
        {
            var userResponse = await _identityService.AuthenticateAsync(command.Email, command.Password, ct);

            if (userResponse.IsError)
            {
                _logger.LogWarning("User Authentication error occured: {@Errors}", userResponse.Errors);

                return userResponse.Errors;
            }

            var generateTokenResult = await _tokenProvider.GenerateJwtTokenAsync(userResponse.Value, ct);

            if (generateTokenResult.IsError)
            {
                _logger.LogError("Generate token error occured: {@Errors}", generateTokenResult.Errors);

                return generateTokenResult.Errors;
            }

            return generateTokenResult.Value;
        }
    }
}
