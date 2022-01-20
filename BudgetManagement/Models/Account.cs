using BudgetManagement.Validations;
using System.ComponentModel.DataAnnotations;

namespace BudgetManagement.Models
{
    public class Account
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength: 50)]
        [FirstLetterCapital]
        public string Name { get; set; }

        [Display(Name = "Account Type")]
        public int AccountTypeId { get; set; }

        public decimal Balance { get; set; }

        [StringLength(maximumLength: 1000)]
        public string Description { get; set; }

        public string AccountType { get; set; }
    }
}
