using BudgetManagement.Models;

namespace BudgetManagement.Interface
{
    public interface IReportsService
    {

        Task<DetailedTransactionsReport> GetTransactionsDetailedReport(int userId, int month, int year, dynamic ViewBag);

        Task<DetailedTransactionsReport> GetDetailedTransactionsReportByAccount(int userId, int accountId, int month, int year, dynamic ViewBag);
        Task<IEnumerable<GetByWeekResult>> GetByWeek(int userId, int month, int year, dynamic ViewBag);
    }
}
