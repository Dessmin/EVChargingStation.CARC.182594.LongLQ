using EVChargingStation.CARC.Domain.LongLQ.Enums;
using System.ComponentModel.DataAnnotations;

namespace EVChargingStation.CARC.Domain.LongLQ.Entities
{
    public class Session : BaseEntity
    {
        [Required]
        public Guid ConnectorId { get; set; }

        [Required]
        public Guid UserId { get; set; }

        public Guid? ReservationLongLQId { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public SessionStatus Status { get; set; } = SessionStatus.Running;

        // State of Charge (SoC) at the start and end of the session
        public decimal? SocStart { get; set; }
        public decimal? SocEnd { get; set; }

        // Energy consumed in kWh during the session
        public decimal? EnergyKwh { get; set; }

        public decimal? Cost { get; set; }

        public Guid? InvoiceTruongNNId { get; set; }

        // Navigation properties
        public Connector Connector { get; set; } = null!;
        public User User { get; set; } = null!;
        public ReservationLongLQ? ReservationLongLQ { get; set; }
        public InvoiceTruongNN? InvoiceTruongNN { get; set; }
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
