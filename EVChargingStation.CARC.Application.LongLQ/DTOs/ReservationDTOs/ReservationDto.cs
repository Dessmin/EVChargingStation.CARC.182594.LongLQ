using EVChargingStation.CARC.Domain.LongLQ.Enums;

namespace EVChargingStation.CARC.Application.LongLQ.DTOs.ReservationDTOs
{
    public class ReservationDto
    {
        public Guid Id { get; set; }
        public string User { get; set; }
        public string StationName { get; set; }
        public ConnectorType ConnectorType { get; set; }
        public decimal MinPowerKw { get; set; }
        public ReservationStatus Status { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
