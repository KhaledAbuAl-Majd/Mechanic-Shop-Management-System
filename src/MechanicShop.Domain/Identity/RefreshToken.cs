using MechanicShop.Domain.Common;
using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Domain.Identity
{
    public sealed class RefreshToken : AuditableEntity
    {
        public string Token { get; } = null!;
        public string UserId { get; } = null!;
        public DateTimeOffset ExpiresOnUtc { get; }
        public bool IsRevoked { get; private set; }

        public bool IsActive(TimeProvider datetime) =>
             !IsRevoked && ExpiresOnUtc >= datetime.GetUtcNow();

        private RefreshToken() { }

        private RefreshToken(Guid id, string token, string userId, DateTimeOffset expiresOnUtc, bool isRevoked) : base(id)
        {
            Token = token;
            UserId = userId;
            ExpiresOnUtc = expiresOnUtc;
            IsRevoked = isRevoked;
        }

        public static Result<RefreshToken> Create(Guid id, string token, string userId, DateTimeOffset expiresOnUtc, TimeProvider datetime)
        {
            if (id == Guid.Empty)
                return RefreshTokenErrors.IdRequired;

            if (string.IsNullOrWhiteSpace(token))
                return RefreshTokenErrors.TokenRequired;

            if (string.IsNullOrWhiteSpace(userId))
                return RefreshTokenErrors.UserIdRequired;

            if (expiresOnUtc <= datetime.GetUtcNow())
                return RefreshTokenErrors.ExpiryInvalid;

            return new RefreshToken(id, token, userId, expiresOnUtc, false);
        }

        public Result<Updated> Revoke()
        {
            if (IsRevoked)
                return RefreshTokenErrors.RefreshTokenAlreadyRevoked;

            IsRevoked = true;

            return Result.Updated;
        }
    }
}
