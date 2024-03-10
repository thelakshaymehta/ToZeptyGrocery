using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ToZeptyUI.Models
{
    public class ResetPasswordViewModel
    {
        [Required (ErrorMessage = "Username is required")]
        [RegularExpression(@"^[a-zA-Z0-9_]*$", ErrorMessage = "Only letters, numbers, and underscores are allowed.")]
        public string UserName { get; set; }


        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^(?:[0-9] ?){6,14}[0-9]$", ErrorMessage = "Invalid phone number format.")]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [MaxLength(255)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$", ErrorMessage = "Password must have at least 8 characters, one uppercase letter, one lowercase letter, and one digit.")]
        public string Password { get; set; }



        [Compare("Password", ErrorMessage = "The password and confirm password do not match.")]
        [DataType(DataType.Password)]
        [MaxLength(255)]
        [Display(Name = "Confirm Password")]
        [Required]
        public string ConfirmPassword { get; set; }
    }
}