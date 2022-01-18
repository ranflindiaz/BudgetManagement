using Microsoft.AspNetCore.Mvc.Rendering;

namespace BudgetManagement.Models
{
    public class CreateAccountViewModel : Account
    {
        public IEnumerable<SelectListItem> AccountTypes { get; set; }
    }
}
