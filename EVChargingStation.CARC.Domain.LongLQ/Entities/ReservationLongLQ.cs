using EVChargingStation.CARC.Domain.LongLQ.Enums;
using System.ComponentModel.DataAnnotations;

namespace EVChargingStation.CARC.Domain.LongLQ.Entities
{
    public class ReservationLongLQ : BaseEntity
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public Guid StationAnhDHVId { get; set; }

        public Guid ConnectorId { get; set; }

        public ConnectorType PreferredConnectorType { get; set; }

        public decimal MinPowerKw { get; set; }

        public PriceType PriceType { get; set; } = PriceType.Free;

        [Required]
        public DateTime StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public ReservationStatus Status { get; set; } = ReservationStatus.Pending;

        // Navigation properties
        public User User { get; set; } = null!;
        public StationAnhDHV StationAnhDHV { get; set; } = null!;
        public Connector? Connector { get; set; }
        public Session? Session { get; set; }
    }
}
