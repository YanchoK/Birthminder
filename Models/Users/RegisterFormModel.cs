using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Birthminder.Models
{
    public class RegisterFormModel
    {
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [RegularExpression(@"[!-~]+", ErrorMessage ="Use only valid symbols")]
        
        public string Password { get; set; }
        [Required(ErrorMessage = "Full Name is required")]
        [RegularExpression(@"[!-~ ]+", ErrorMessage = "Use only valid symbols")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Birth Date is required")]
        public DateTime BirthDate { get; set; }

        [Required(ErrorMessage = "Team Choose is required")]
        public int TeamId { get; set; }
    }
}
