using EVChargingStation.CARC.Application.LongLQ.DTOs.AuthenDTOs;
using EVChargingStation.CARC.Application.LongLQ.DTOs.UserDTOs;
using EVChargingStation.CARC.Application.LongLQ.Interfaces;
using EVChargingStation.CARC.Application.LongLQ.Utils;
using EVChargingStation.CARC.Domain.LongLQ.Entities;
using EVChargingStation.CARC.Domain.LongLQ.Enums;
using EVChargingStation.CARC.Infrastructure.LongLQ.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EVChargingStation.CARC.Application.LongLQ.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public AuthService(IUnitOfWork unitOfWork, ILogger<AuthService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<UserDto> RegisterUserAsync(UserRegistrationDto userRegistrationDto)
        {
            try
            {
                _logger.LogInformation("Attempting to register user: {Email}", userRegistrationDto.Email);

                var checkExistingUser = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Email == userRegistrationDto.Email);
                if (checkExistingUser != null)
                {
                    _logger.LogWarning("Registration failed: Email {Email} already exists.", userRegistrationDto.Email);
                    throw new Exception("Email already exists.");
                }

                var hashedPassword = new PasswordHasher().HashPassword(userRegistrationDto.Password);

                var user = new User
                {
                    FirstName = userRegistrationDto.FirstName,
                    LastName = userRegistrationDto.LastName,
                    Email = userRegistrationDto.Email,
                    PasswordHash = hashedPassword,
                    DateOfBirth = userRegistrationDto.DateOfBirth,
                    Gender = userRegistrationDto.Gender,
                    Phone = userRegistrationDto.Phone,
                    Address = userRegistrationDto.Address,
                    Role = RoleType.Driver,
                    Status = UserStatus.Active,
                };
                await _unitOfWork.Users.AddAsync(user);
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("User registered successfully: {Email}", userRegistrationDto.Email);
                return new UserDto
                {
                    UserId = user.LongLQID,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Phone = user.Phone,
                    Address = user.Address,
                    Role = user.Role,
                    Status = user.Status,
                    CreatedAt = user.CreatedAt
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while registering user: {Email}", userRegistrationDto.Email);
                throw;
            }
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto loginDto, IConfiguration configuration)
        {
            try
            {
                _logger.LogInformation("Attempting to log in user: {Email}", loginDto.Email);
                _logger.LogDebug("Login details: {@LoginDto}", loginDto);
                var user = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);
                if (user == null)
                {
                    _logger.LogWarning("Login failed: User with email {Email} not found.", loginDto.Email);
                    throw new Exception("User not found.");
                }

                var passwordHasher = new PasswordHasher();

                if (!passwordHasher.VerifyPassword(loginDto.Password!, user.PasswordHash))
                {
                    _logger.LogWarning("Login failed: Invalid password for email {Email}.", loginDto.Email);
                    throw new Exception("Invalid password.");
                }

                if (user.Status != UserStatus.Active)
                {
                    _logger.LogWarning("Login failed: User with email {Email} is not active.", loginDto.Email);
                    throw new Exception("User is not active.");
                }

                _logger.LogInformation("User {Email} authenticated successfully.", loginDto.Email);

                var accessToken = JwtUtils.GenerateJwtToken(
                    user.LongLQID,
                    user.Email,
                    user.Role.ToString(),
                    configuration,
                    TimeSpan.FromMinutes(15)
                );

                var refreshToken = Guid.NewGuid().ToString();
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

                await _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Login successful for user: {Email}", loginDto.Email);
                return new LoginResponseDto
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while logging in user: {Email}", loginDto.Email);
                throw;
            }
        }

        public async Task<bool> LogoutAsync(Guid userId)
        {
            try
            {
                _logger.LogInformation("Attempting to log out user: {UserId}", userId);
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("Logout failed: User with ID {UserId} not found.", userId);
                    throw new Exception("User not found.");
                }

                if (string.IsNullOrEmpty(user.RefreshToken))
                {
                    _logger.LogWarning("Logout failed: No active session for user ID {UserId}.", userId);
                    throw new Exception("No active session found.");
                }

                user.RefreshToken = null;
                user.RefreshTokenExpiryTime = null;
                await _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("User {UserId} logged out successfully.", userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while logging out user: {UserId}", userId);
                throw;
            }
        }

        public async Task<LoginResponseDto?> RefreshTokenAsync(TokenRefreshRequestDto refreshTokenDto, IConfiguration configuration)
        {
            try
            {
                _logger.LogInformation("Attempting to refresh token for user");
                var user = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshTokenDto.RefreshToken);
                if (user == null || user.RefreshToken != refreshTokenDto.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                {
                    _logger.LogWarning("Token refresh failed for user");
                    throw new Exception("Invalid refresh token.");
                }

                if (user.RefreshTokenExpiryTime < DateTime.UtcNow)
                {
                    _logger.LogWarning("Token refresh failed: Refresh token expired for user");
                    throw new Exception("Refresh token has expired.");
                }

                var roleName = user.Role.ToString();
                var newAccessToken = JwtUtils.GenerateJwtToken(
                    user.LongLQID,
                    user.Email,
                    user.Role.ToString(),
                    configuration,
                    TimeSpan.FromHours(1)
                );

                var newRefreshToken = Guid.NewGuid().ToString();
                user.RefreshToken = newRefreshToken;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

                await _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Token refreshed successfully for user: {UserId}", user.LongLQID);
                return new LoginResponseDto
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while refreshing token for user");
                throw;
            }
        }
    }
}
