using EVChargingStation.CARC.Application.LongLQ.DTOs.AuthenDTOs;
using EVChargingStation.CARC.Application.LongLQ.DTOs.UserDTOs;
using EVChargingStation.CARC.Application.LongLQ.Interfaces;
using EVChargingStation.CARC.Application.LongLQ.Utils;
using EVChargingStation.CARC.Infrastructure.LongLQ.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace EVChargingStation.CARC.WebAPI.LongLQ.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;
        private readonly IClaimsService _claimsService;
        public AuthController(IAuthService authService, IConfiguration configuration, IClaimsService claimsService)
        {
            _authService = authService;
            _configuration = configuration;
            _claimsService = claimsService;
        }

        [HttpPost("register")]
        [SwaggerOperation(
            Summary = "Register a new user",
            Description = "Creates a new user account with the provided registration information."
        )]
        [ProducesResponseType(typeof(ApiResult<UserDto>), 200)]
        [ProducesResponseType(typeof(ApiResult<UserDto>), 400)]
        public async Task<IActionResult> Register([FromBody] UserRegistrationDto registrationDto)
        {
            try
            {
                var result = await _authService.RegisterUserAsync(registrationDto);
                return Ok(ApiResult<UserDto>.Success(result!, "200", "Registered successfully."));
            }
            catch (Exception ex)
            {
                var statusCode = ExceptionUtils.ExtractStatusCode(ex);
                var errorResponse = ExceptionUtils.CreateErrorResponse<UserDto>(ex);
                return StatusCode(statusCode, errorResponse);
            }
        }

        [HttpPost("login")]
        [SwaggerOperation(
            Summary = "User login",
            Description = "Authenticates a user and returns a JWT token upon successful login."
        )]
        [ProducesResponseType(typeof(ApiResult<LoginResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResult<LoginResponseDto>), 400)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginDto)
        {
            try
            {
                var result = await _authService.LoginAsync(loginDto, _configuration);
                return Ok(ApiResult<LoginResponseDto>.Success(result!, "200", "Login successful."));
            }
            catch (Exception ex)
            {
                var statusCode = ExceptionUtils.ExtractStatusCode(ex);
                var errorResponse = ExceptionUtils.CreateErrorResponse<LoginResponseDto>(ex);
                return StatusCode(statusCode, errorResponse);
            }
        }

        [HttpPost("logout")]
        [SwaggerOperation(
            Summary = "User logout",
            Description = "Logs out the user by invalidating their current session."
        )]
        [ProducesResponseType(typeof(ApiResult<bool>), 200)]
        [ProducesResponseType(typeof(ApiResult<bool>), 400)]
        [ProducesResponseType(typeof(ApiResult<bool>), 500)]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var userId = _claimsService.GetCurrentUserId;
                var result = await _authService.LogoutAsync(userId);
                return Ok(ApiResult<object>.Success(result!, "200", "Loged out successfully."));
            }
            catch (Exception ex)
            {
                var statusCode = ExceptionUtils.ExtractStatusCode(ex);
                var errorResponse = ExceptionUtils.CreateErrorResponse<object>(ex);
                return StatusCode(statusCode, errorResponse);
            }
        }

        [HttpPost("refresh-token")]
        [SwaggerOperation(
            Summary = "Refresh JWT token",
            Description = "Refresh JWT access token using a valid refresh token."
        )]
        [ProducesResponseType(typeof(ApiResult<LoginResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResult<object>), 400)]
        [ProducesResponseType(typeof(ApiResult<object>), 401)]
        [ProducesResponseType(typeof(ApiResult<object>), 500)]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRefreshRequestDto requestToken)
        {
            try
            {
                var result = await _authService.RefreshTokenAsync(requestToken, _configuration);
                return Ok(ApiResult<object>.Success(result!, "200", "Refresh Token successfully"));
            }
            catch (Exception ex)
            {
                var statusCode = ExceptionUtils.ExtractStatusCode(ex);
                var errorResponse = ExceptionUtils.CreateErrorResponse<object>(ex);
                return StatusCode(statusCode, errorResponse);
            }
        }
    }
}
