using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FleetManagementSystem.Models
{
    [Table("Users")]
    public class User_Details
    {
        [Key]
        public int UserId { get; set; }

        [Required(ErrorMessage = "First name is required")]
        [RegularExpression("^[A-Za-z]+$", ErrorMessage = "First name must contain only letters.")]
        [MaxLength(50, ErrorMessage = "First name can't exceed 50 characters")]
        public string FirstName { get; set; }=string.Empty;

        [Required(ErrorMessage = "Last name is required")]
        [RegularExpression("^[A-Za-z]+$", ErrorMessage = "First name must contain only letters.")]
        [MaxLength(50, ErrorMessage = "Last name can't exceed 50 characters")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Please enter a valid 10-digit mobile number.")]
        [MaxLength(10, ErrorMessage = "Phone number can't exceed 10 digits")]
        public string PhoneNumber { get; set; }=string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } =string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$",
            ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Role is required")]
        [MaxLength(20, ErrorMessage = "Role can't exceed 20 characters")]
        [RegularExpression("^(Customer|Driver|Admin)$", ErrorMessage = "Role must be Customer, Driver, or Admin")]
        public string Role { get; set; }= string.Empty; 
    }
}
