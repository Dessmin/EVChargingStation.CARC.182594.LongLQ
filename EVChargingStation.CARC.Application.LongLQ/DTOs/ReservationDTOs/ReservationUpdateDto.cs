using EVChargingStation.CARC.Domain.LongLQ.Enums;

namespace EVChargingStation.CARC.Application.LongLQ.DTOs.ReservationDTOs
{
    public class ReservationUpdateDto
    {
        public Guid? StationId { get; set; }
        public Guid? ConnectorId { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public ReservationStatus? Status { get; set; }
    }
}