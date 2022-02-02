namespace BudgetManagement.Models
{
    public class GetTransactionsParameterByUser
    {
        public int UserId { get; set; }
        public DateTime InitialDate { get; set; }
        public DateTime EndDate { get; set; }

    }
}
