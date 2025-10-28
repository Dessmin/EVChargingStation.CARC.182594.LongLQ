using System.ComponentModel.DataAnnotations;

namespace EVChargingStation.CARC.Domain.LongLQ.Entities
{
    public class Location : BaseEntity
    {
        [MaxLength(100)]
        public string? Name { get; set; }

        [Required]
        public string Address { get; set; } = string.Empty;

        [Required]
        [Range(-90, 90)]
        public decimal Latitude { get; set; }

        [Required]
        [Range(-180, 180)]
        public decimal Longitude { get; set; }

        [MaxLength(100)]
        public string? City { get; set; }

        [MaxLength(100)]
        public string? StateProvince { get; set; }

        [MaxLength(100)]
        public string? Country { get; set; }

        [MaxLength(50)]
        public string? Timezone { get; set; }

        // Navigation Property
        public ICollection<StationAnhDHV> StationAnhDHV { get; set; } = new List<StationAnhDHV>();
    }
}
