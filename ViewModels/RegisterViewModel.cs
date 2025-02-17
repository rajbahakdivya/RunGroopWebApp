using CloudinaryDotNet.Actions;
using System.ComponentModel.DataAnnotations;

namespace RunGroopWebApp.ViewModels
{
    public class RegisterViewModel
    {
        [Display(Name = "Email address is required")]
        [Required(ErrorMessage = "Email address is required")]
        public string? EmailAddress { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Display(Name ="Confirm password")]
        [Required(ErrorMessage ="Confirm  password is required")]
        [DataType(DataType.Password)]
        [Compare("Password",ErrorMessage = "Password do not match")]
        public string? ConfirmPassword { get; set; }
    }
}