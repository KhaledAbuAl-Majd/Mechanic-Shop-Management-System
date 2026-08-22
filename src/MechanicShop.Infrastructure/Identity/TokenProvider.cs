using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Common.Settings;
using MechanicShop.Application.Features.Identity.Dtos;
using MechanicShop.Application.Features.Identity.Interfaces;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Identity;
using Microsoft.IdentityModel.Tokens;

namespace MechanicShop.Infrastructure.Identity
{
    public class TokenProvider(IAppDbContext context, JwtSettings jwtSettings, TimeProvider datetime) : ITokenProvider
    {
        private readonly IAppDbContext _context = context;
        private readonly JwtSettings _jwtSettings = jwtSettings;
        private readonly TimeProvider _datetime = datetime;

        public async Task<Result<TokenDto>> GenerateJwtTokenAsync(AppUserDto user, CancellationToken ct = default)
        {
            return await CreateAsync(user, ct);
        }

        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            //validate token manual (same as authentication middleware)

            var tokenValidatorParamters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret)),
                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtSettings.Audience,
                ValidateLifetime = false,//that's for expired token also
                ClockSkew = TimeSpan.Zero
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidatorParamters, out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token.");
            }

            return principal;
        }

        public async Task<Result<TokenDto>> CreateAsync(AppUserDto user, CancellationToken ct = default)
        {
            var expires = _datetime.GetUtcNow().AddMinutes(_jwtSettings.TokenExpirationInMinutes);

            var claims = new List<Claim>
            {
                new (JwtRegisteredClaimNames.Sub,user.UserId!),
                new (JwtRegisteredClaimNames.Email,user.Email),
            };

            foreach (var role in user.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expires.DateTime,
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret)),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var securityToken = tokenHandler.CreateToken(descriptor);

            var refreshTokenResult = RefreshToken.Create(
                Guid.NewGuid(),
                GenerateRefreshToken(),
                user.UserId,
                _datetime.GetUtcNow().AddDays(_jwtSettings.RefreshTokenExpirationInDays),
                _datetime);

            if (refreshTokenResult.IsError)
                return refreshTokenResult.Errors;

            var refreshToken = refreshTokenResult.Value;

            _context.RefreshTokens.Add(refreshToken);

            await _context.SaveChangesAsync(ct);

            return new TokenDto(tokenHandler.WriteToken(securityToken), refreshToken.Token, expires.DateTime);
        }

        private static string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        }
    }
}
