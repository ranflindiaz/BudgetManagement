using System.ComponentModel.DataAnnotations;

namespace BudgetManagement.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [StringLength(maximumLength: 50, ErrorMessage = "Can not be higher than {1} characteres")]
        public string Name { get; set; }

        [Display(Name = "Operation Type")]
        public OperationType OperationTypeId { get; set; }

        public int UserId { get; set; }
    }
}
