namespace BudgetManagement.Models
{
    public class GetByWeekResult
    {
        public int Week { get; set; }

        public decimal Amount { get; set; }

        public OperationType OperationTypeId { get; set; }

        public decimal Income { get; set; }

        public decimal Spending { get; set; }

        public DateTime InitialDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
