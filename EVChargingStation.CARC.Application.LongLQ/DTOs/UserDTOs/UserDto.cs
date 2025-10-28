using EVChargingStation.CARC.Domain.LongLQ.Enums;

namespace EVChargingStation.CARC.Application.LongLQ.DTOs.UserDTOs
{
    public class UserDto
    {
        public Guid UserId { get; set; }

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public string? Password { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public Gender? Gender { get; set; }

        public string Email { get; set; } = string.Empty;

        public string? Phone { get; set; }

        public string? Address { get; set; }

        public RoleType Role { get; set; }

        public UserStatus Status { get; set; } = UserStatus.Active;

        public DateTime CreatedAt { get; set; }
    }
}
