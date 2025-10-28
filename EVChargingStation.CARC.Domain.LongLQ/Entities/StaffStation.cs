using System.ComponentModel.DataAnnotations;

namespace EVChargingStation.CARC.Domain.LongLQ.Entities
{
    public class StaffStation : BaseEntity
    {
        [Required]
        public Guid StaffUserId { get; set; }

        [Required]
        public Guid StationAnhDHVId { get; set; }

        // Navigation properties
        public User StaffUser { get; set; } = null!;
        public StationAnhDHV StationAnhDHV { get; set; } = null!;
    }
}
