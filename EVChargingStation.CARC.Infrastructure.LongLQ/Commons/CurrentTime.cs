using EVChargingStation.CARC.Infrastructure.LongLQ.Interfaces;

namespace EVChargingStation.CARC.Infrastructure.LongLQ.Commons
{
    public class CurrentTime : ICurrentTime
    {
        public DateTime GetCurrentTime()
        {
            return DateTime.UtcNow; // Đảm bảo trả về thời gian UTC
        }
    }
}
