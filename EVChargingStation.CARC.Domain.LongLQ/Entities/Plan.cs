using EVChargingStation.CARC.Domain.LongLQ.Enums;
using System.ComponentModel.DataAnnotations;

namespace EVChargingStation.CARC.Domain.LongLQ.Entities
{
    public class Plan : BaseEntity
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public PlanType Type { get; set; }

        [Range(0, 1000000)]
        public decimal? Price { get; set; }

        [Range(0, 10000)]
        public decimal? MaxDailyKwh { get; set; }

        // Navigation property
        public ICollection<UserPlanHoaHTT> UserPlanHoaHTT { get; set; } = new List<UserPlanHoaHTT>();
    }
}
