namespace Sharedkernel
{
    public interface IDateTimeProvider
    {
        public DateTime UtcNow { get; }

        public string TimeStamp { get; }
    }
}
