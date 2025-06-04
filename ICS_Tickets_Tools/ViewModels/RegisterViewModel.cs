using System.ComponentModel.DataAnnotations;

namespace ICS_Tickets_Tools.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Please Enter Your Name")]
        [RegularExpression(@"^[A-Z][a-zA-Z]*$", ErrorMessage = "Name must start with a capital letter and contain no spaces.")]
        public string EmployeeName { get; set; }

        [Required(ErrorMessage = "Please Enter Your Employee Code")]
        public string EmployeeCode { get; set; }

		[Required(ErrorMessage = "Please Enter Your Phone Number")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Please enter valid number")]
        public string PhoneNumber { get; set; }

		[Required(ErrorMessage = "Please select a Department.")]
        public string Department { get; set; }

        [Required(ErrorMessage = "Please select a Designation.")]
        public string Designation { get; set; }

        [Required(ErrorMessage = "Please Enter your password.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please confirm your password.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }

		[Required(ErrorMessage = "Please select a role.")]
		public string Role { get; set; }
	}
}
