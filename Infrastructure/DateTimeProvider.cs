using Sharedkernel;

namespace Infrastructure
{
    internal sealed class DateTimeProvider : IDateTimeProvider
    {
        public DateTime UtcNow => DateTime.UtcNow;

        public string TimeStamp => UtcNow.ToString("yyMMddHHmmss");
    }
}
