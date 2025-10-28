using System.ComponentModel;

namespace EVChargingStation.CARC.Application.LongLQ.DTOs.AuthenDTOs
{
    public class LoginRequestDto
    {
        [DefaultValue("admin@gmail.com")]
        public required string? Email { get; set; }

        [DefaultValue("1@")]
        public required string? Password { get; set; }
    }
}
