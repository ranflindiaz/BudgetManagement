namespace BudgetManagement.Models
{
    public class WeeklyReportViewModel
    {
        public decimal Income => WeeklyTransactions.Sum(x => x.Income);

        public decimal Spending => WeeklyTransactions.Sum(x => x.Spending);

        public decimal Total => Income - Spending;

        public DateTime ReferenceDate { get; set; }

        public IEnumerable<GetByWeekResult> WeeklyTransactions { get; set; }
    }
}
