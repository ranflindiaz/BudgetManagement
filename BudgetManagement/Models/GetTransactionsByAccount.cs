namespace BudgetManagement.Models
{
    public class GetTransactionsByAccount
    {
        public int UserId { get; set; }

        public int AccountId { get; set; }

        public DateTime InitialDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
