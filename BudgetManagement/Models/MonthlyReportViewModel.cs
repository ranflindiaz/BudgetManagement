namespace BudgetManagement.Models
{
    public class MonthlyReportViewModel
    {
        public IEnumerable<GetByMonthResult> MonthlyTransactions { get; set; }

        public decimal Income => MonthlyTransactions.Sum(x => x.Income);

        public decimal Spending => MonthlyTransactions.Sum(x => x.Spending);

        public decimal Total => Income - Spending;

        public int Year { get; set; }

    }
}
