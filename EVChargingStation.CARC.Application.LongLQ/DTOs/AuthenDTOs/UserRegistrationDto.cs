using EVChargingStation.CARC.Domain.LongLQ.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EVChargingStation.CARC.Application.LongLQ.DTOs.AuthenDTOs
{
    public class UserRegistrationDto
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [DefaultValue("ch1mple@gmail.com")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters.")]
        [DataType(DataType.Password)]
        [DefaultValue("Ch1mple@")]
        public required string Password { get; set; }

        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
        [DefaultValue("Ch1mple")]
        public string FirstName { get; set; }

        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
        [DefaultValue("User")]
        public string LastName { get; set; }

        [DefaultValue("2002-03-06T00:00:00Z")]
        public DateTime? DateOfBirth { get; set; }

        public Gender? Gender { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number.")]
        [RegularExpression(@"^0[0-9]{9}$", ErrorMessage = "Phone number must be 10 digits and start with 0.")]
        [DefaultValue("0393734206")]
        public required string Phone { get; set; }

        [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters.")]
        public string? Address { get; set; }

    }
}
