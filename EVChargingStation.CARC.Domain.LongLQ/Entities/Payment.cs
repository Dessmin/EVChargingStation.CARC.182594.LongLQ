using EVChargingStation.CARC.Domain.LongLQ.Enums;
using System.ComponentModel.DataAnnotations;

namespace EVChargingStation.CARC.Domain.LongLQ.Entities
{
    public class Payment : BaseEntity
    {
        [Required]
        [Range(0, 1000000)]
        public decimal Amount { get; set; }

        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        public string? Note { get; set; }

        public Guid? InvoiceTruongNNId { get; set; }

        public Guid? SessionId { get; set; }

        public Guid UserId { get; set; }

        // Navigation properties
        public User User { get; set; } = null!;
        public InvoiceTruongNN? InvoiceTruongNN { get; set; }
        public Session? Session { get; set; }
    }
}
