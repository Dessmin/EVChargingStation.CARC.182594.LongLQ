using EVChargingStation.CARC.Domain.LongLQ.Enums;
using System.ComponentModel.DataAnnotations;

namespace EVChargingStation.CARC.Domain.LongLQ.Entities
{
    public class StationAnhDHV : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public Guid LocationId { get; set; }

        public StationStatus Status { get; set; } = StationStatus.Online;

        // Navigation Property
        public Location Location { get; set; } = null!;
        public ICollection<Connector> Connectors { get; set; } = new List<Connector>();
        public ICollection<StaffStation> StaffStations { get; set; } = new List<StaffStation>();
        public ICollection<Recommendation> Recommendations { get; set; } = new List<Recommendation>();
        public ICollection<ReservationLongLQ> ReservationLongLQ { get; set; } = new List<ReservationLongLQ>();
    }
}
