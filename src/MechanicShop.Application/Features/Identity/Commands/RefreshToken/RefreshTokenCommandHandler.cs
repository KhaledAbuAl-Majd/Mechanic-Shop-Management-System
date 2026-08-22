using System.Security.Claims;
using MechanicShop.Application.Common.Errors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.Identity.Dtos;
using MechanicShop.Application.Features.Identity.Interfaces;
using MechanicShop.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.Identity.Commands.RefreshToken
{
    public sealed class RefreshTokenCommandHandler(
        ILogger<RefreshTokenCommandHandler> logger,
        IIdentityService identityService,
        IAppDbContext context,
        ITokenProvider tokenProvider,
        TimeProvider datetime) : IRequestHandler<RefreshTokenCommand, Result<TokenDto>>
    {
        private readonly ILogger<RefreshTokenCommandHandler> _logger = logger;
        private readonly IIdentityService _identityService = identityService;
        private readonly IAppDbContext _context = context;
        private readonly ITokenProvider _tokenProvider = tokenProvider;
        private readonly TimeProvider _datetime = datetime;

        public async Task<Result<TokenDto>> Handle(RefreshTokenCommand command, CancellationToken ct)
        {
            var principal = _tokenProvider.GetPrincipalFromExpiredToken(command.ExpiredAccessToken);

            if (principal is null)
            {
                _logger.LogWarning("Expired acces token is not valid");

                return ApplicationErrors.ExpiredAccessTokenInvalid;
            }

            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId is null)
            {
                _logger.LogWarning("Invalid userId claim");

                return ApplicationErrors.UserIdClaimInvalid;
            }

            var getUserResult = await _identityService.GetUserByIdAsync(userId, ct);

            if (getUserResult.IsError)
            {
                _logger.LogWarning("Get user by id error occurred: {@Errors}", getUserResult.Errors);
                return getUserResult.Errors;
            }

            var refreshToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(
                rt => rt.Token == command.RefreshToken && rt.UserId == userId,
                ct);

            if (refreshToken is null || !refreshToken.IsActive(_datetime))
            {
                _logger.LogWarning("Refresh token is invalid or expired for user {UserId}", userId);

                return ApplicationErrors.RefreshTokenExpired;
            }

            var generateTokenResult = await _tokenProvider.GenerateJwtTokenAsync(getUserResult.Value, ct);

            if (generateTokenResult.IsError)
            {
                _logger.LogError("Generate token error occurred: {@Errors}", generateTokenResult.Errors);

                return generateTokenResult.Errors;
            }

            var revokeTokenResult = refreshToken.Revoke();

            if (revokeTokenResult.IsError)
            {
                _logger.LogWarning("Revoke token with Id {@tokenId}, error occured: {@Errors}", refreshToken.Id, revokeTokenResult.Errors);

                return revokeTokenResult.Errors;
            }

            await _context.SaveChangesAsync(ct);

            return generateTokenResult.Value;
        }
    }
}
