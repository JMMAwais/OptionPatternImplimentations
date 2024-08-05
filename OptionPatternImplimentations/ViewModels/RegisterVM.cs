using System.ComponentModel.DataAnnotations;

namespace OptionPatternImplimentations.ViewModels
{
    public class RegisterVM
    {
        [Required(ErrorMessage = "Name is Required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is Required")]
        [EmailAddress]
        public string Email { get; set; }


        [Required(ErrorMessage = "Password is Required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmed Password")]
        [Compare("Password", ErrorMessage = "The Password and Confirm Password do not match")]
        public string ConfirmedPassword { get; set; }
    }
}
