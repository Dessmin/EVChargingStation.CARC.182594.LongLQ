using EVChargingStation.CARC.Application.LongLQ.DTOs.AuthenDTOs;
using EVChargingStation.CARC.Application.LongLQ.DTOs.UserDTOs;
using Microsoft.Extensions.Configuration;

namespace EVChargingStation.CARC.Application.LongLQ.Interfaces
{
    public interface IAuthService
    {
        Task<UserDto?> RegisterUserAsync(UserRegistrationDto registrationDto);

        Task<LoginResponseDto?> LoginAsync(LoginRequestDto loginDto, IConfiguration configuration);

        Task<bool> LogoutAsync(Guid userId);

        Task<LoginResponseDto?> RefreshTokenAsync(TokenRefreshRequestDto refreshTokenDto, IConfiguration configuration);
    }
}
