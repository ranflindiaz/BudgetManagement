using System.ComponentModel.DataAnnotations;

namespace BudgetManagement.Models
{
    public class LoginViewModel
    {
        public bool Remember { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [EmailAddress(ErrorMessage = "Field should be a valid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        public string Password { get; set; }
    }
}
