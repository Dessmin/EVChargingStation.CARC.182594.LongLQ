using EVChargingStation.CARC.Domain.LongLQ.Enums;
using EVChargingStation.CARC.Application.LongLQ.DTOs.StationDTOs;

namespace EVChargingStation.CARC.Application.LongLQ.DTOs.ReservationDTOs
{
    public class ReservationDto
    {
        public Guid Id { get; set; }
        public string User { get; set; }

        public ReservationStatus Status { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public DateTime CreatedAt { get; set; }

        // Include station details (connectors) with the reservation (contains station id, name, connectors)
        public StationConnectorsDto Station { get; set; } = new StationConnectorsDto();
    }
}
