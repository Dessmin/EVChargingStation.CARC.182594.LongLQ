namespace EVChargingStation.CARC.Application.LongLQ.DTOs.AuthenDTOs
{
    public class LoginResponseDto
    {
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }
    }
}
