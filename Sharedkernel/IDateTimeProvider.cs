namespace Sharedkernel
{
    public interface IDateTimeProvider
    {
        public DateTime UtcNow { get; }
        public DateOnly DateNow { get; }

        public string TimeStamp { get; }

        public string DateStamp { get; }
    }
}
