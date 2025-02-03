using Sharedkernel;

namespace Infrastructure
{
    internal sealed class DateTimeProvider : IDateTimeProvider
    {
        public DateTime UtcNow => DateTime.UtcNow;

        public DateOnly DateNow => DateOnly.FromDateTime(UtcNow);

        public string TimeStamp => UtcNow.ToString("yyMMddHHmmss");

        public string DateStamp => UtcNow.ToString("yyyyMMdd");

        public int DayOfTheYear => DateNow.DayOfYear;
    }
}
