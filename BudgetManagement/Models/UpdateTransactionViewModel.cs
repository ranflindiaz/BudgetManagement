namespace BudgetManagement.Models
{
    public class UpdateTransactionViewModel : CreateTransactionViewModel
    {
        public int PreviousAccountId { get; set; }

        public decimal PreviousAmount { get; set; }

        public string ReturnUrl { get; set; }
    }
}
