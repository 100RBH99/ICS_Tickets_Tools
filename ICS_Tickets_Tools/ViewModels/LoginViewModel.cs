using System.ComponentModel.DataAnnotations;

namespace ICS_Tickets_Tools.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Please Enter Your Employee Name")]
        public string EmployeeName { get; set; }

        [Required(ErrorMessage = "Please Enter your password.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        //[Required(ErrorMessage = "Please select a role.")]
        //public string Role { get; set; }

        [Required(ErrorMessage = "Please Select reCAPTCHA")]
        public string RecaptchaToken { get; set; }

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }

    }
}
