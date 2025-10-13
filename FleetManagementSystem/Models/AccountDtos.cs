using System.ComponentModel.DataAnnotations;

namespace FleetManagementSystem.Models
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "First name is required")]
        [RegularExpression("^[A-Za-z]+$", ErrorMessage = "First name must contain only letters.")]
        [MaxLength(50, ErrorMessage = "First name can't exceed 50 characters")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [RegularExpression("^[A-Za-z]+$", ErrorMessage = "First name must contain only letters.")]
        [MaxLength(50, ErrorMessage = "Last name can't exceed 50 characters")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Please enter a valid 10-digit mobile number.")]
        [MaxLength(10, ErrorMessage = "Phone number can't exceed 10 digits")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100)]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$",
           ErrorMessage = "Please enter strong password")]
        public string Password { get; set; }
    }

        public class LoginDto
        {
            [Required(ErrorMessage = "Email is required")]
            [EmailAddress(ErrorMessage = "Invalid email address")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Password is required")]
            public string Password { get; set; }
        }
    

    public class TokenResponse
    {
        public required string Token { get; set; }
    }
}
