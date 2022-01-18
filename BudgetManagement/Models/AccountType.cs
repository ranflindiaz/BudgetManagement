using BudgetManagement.Validations;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace BudgetManagement.Models
{
    public class AccountType : IValidatableObject
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [FirstLetterCapital]
        [Remote(action: "VerifyExistAccountType", controller: "AccountTypes")]
        public string Name { get; set; }

        public int UserId { get; set; }

        public int Orden { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Name != null && Name.Length > 0)
            {
                var firstLetter = Name[0].ToString();

                if (firstLetter != firstLetter.ToUpper())
                {
                    yield return new ValidationResult("The first letter have to be uppercase", new[] { nameof(Name) });
                }
            }
        }
    }
}
