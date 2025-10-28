using EVChargingStation.CARC.Domain.LongLQ.Enums;
using System.ComponentModel.DataAnnotations;

namespace EVChargingStation.CARC.Domain.LongLQ.Entities
{
    public class VehicleHuyPD : BaseEntity
    {
        [Required]
        [MaxLength(50)]
        public string Make { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Model { get; set; } = string.Empty;

        public int? Year { get; set; }

        [MaxLength(20)]
        public string? LicensePlate { get; set; }

        [Required]
        public ConnectorType ConnectorType { get; set; }

        // Foreign key
        public Guid UserId { get; set; }

        // Navigation property
        public User User { get; set; } = null!;
    }
}
