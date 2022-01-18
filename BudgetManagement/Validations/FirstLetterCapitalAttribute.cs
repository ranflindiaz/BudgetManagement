using System.ComponentModel.DataAnnotations;

namespace BudgetManagement.Validations
{
    public class FirstLetterCapitalAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return ValidationResult.Success;
            }

            var firstLetterCapital = value.ToString()[0].ToString();

            if (firstLetterCapital != firstLetterCapital.ToUpper())
            {
                return new ValidationResult("La primera letra debe ser mayuscula");
            }

            return ValidationResult.Success;
        }
    }
}
