using System.ComponentModel.DataAnnotations;

namespace EVChargingStation.CARC.Application.LongLQ.DTOs.ReservationDTOs
{
    public class ReservationCreateDto
    {
        [Required]
        public Guid StationId { get; set; }
        [Required]
        public Guid ConnectorId { get; set; }
        [Required]
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}
