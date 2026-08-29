namespace MechanicShop.Tests.Common
{
    public class FakeTimeProvider : TimeProvider
    {
        private DateTimeOffset _utcNow;

        public void SetUtcNow(DateTimeOffset utcNow)
        {
            _utcNow = utcNow;
        }

        public override DateTimeOffset GetUtcNow()
        {
            return _utcNow;
        }

        public override long GetTimestamp()
        {
            return _utcNow.ToUnixTimeMilliseconds();
        }
    }
}
