using BudgetManagement.Interface;
using BudgetManagement.Models;

namespace BudgetManagement.Services
{
    public class ReportsService : IReportsService
    {
        private readonly ITransactionsRepository _transactionsRepository;
        private readonly HttpContext _httpContext;

        public ReportsService(ITransactionsRepository transactionsRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _transactionsRepository = transactionsRepository;
            _httpContext = httpContextAccessor.HttpContext;
        }

        public async Task<IEnumerable<GetByWeekResult>> GetByWeek(int userId, int month, int year, dynamic ViewBag)
        {
            (DateTime initialDate, DateTime endDate) = InitialDateandEndGenerator(month, year);

            var parameter = new GetTransactionsParameterByUser()
            {
                UserId = userId,
                InitialDate = initialDate,
                EndDate = endDate
            };

            AssigneViewBagValues(ViewBag, initialDate);
            var model = await _transactionsRepository.GetByWeek(parameter);
            return model;
        }

        public async Task<DetailedTransactionsReport> 
            GetTransactionsDetailedReport(int userId, int month,
            int year, dynamic ViewBag)
        {
            (DateTime initialDate, DateTime endDate) = InitialDateandEndGenerator(month, year);

            var parameter = new GetTransactionsParameterByUser()
            {
                UserId = userId,
                InitialDate = initialDate,
                EndDate = endDate
            };

            var transactions = await _transactionsRepository.GetByUserId(parameter);

            var model = GetTransactionsDetailedReport(initialDate, endDate, transactions);

            AssigneViewBagValues(ViewBag, initialDate);

            return model;

        }

        public async Task<DetailedTransactionsReport> GetDetailedTransactionsReportByAccount(int userId, int accountId, int month, int year, dynamic ViewBag)
        {
            (DateTime initialDate, DateTime endDate) = InitialDateandEndGenerator(month, year);

            var getTransactionsByAccount = new GetTransactionsByAccount()
            {
                AccountId = accountId,
                UserId = userId,
                InitialDate = initialDate,
                EndDate = endDate
            };

            var transactions = await _transactionsRepository
                .GetByAccoundId(getTransactionsByAccount);

            var model = GetTransactionsDetailedReport(initialDate, endDate, transactions);
            AssigneViewBagValues(ViewBag, initialDate);

            return model;
        }

        private void AssigneViewBagValues(dynamic ViewBag, DateTime initialDate)
        {
            ViewBag.previousMonth = initialDate.AddMonths(-1).Month;
            ViewBag.previousYear = initialDate.AddMonths(-1).Year;
            ViewBag.nextMonth = initialDate.AddMonths(1).Month;
            ViewBag.nextYear = initialDate.AddMonths(1).Year;
            ViewBag.returnUrl = _httpContext.Request.Path + _httpContext.Request.QueryString;
        }

        private static DetailedTransactionsReport GetTransactionsDetailedReport(DateTime initialDate, DateTime endDate, IEnumerable<Transaction> transactions)
        {
            var model = new DetailedTransactionsReport();

            //Sorting up transactions by TransactionDate
            var transactionsByDate = transactions.OrderByDescending(x => x.TransactionDate)
                .GroupBy(x => x.TransactionDate)
                .Select(group => new DetailedTransactionsReport.TransactionsByDate()
                {
                    TransactionDate = group.Key,
                    Transactions = group.AsEnumerable()
                });

            model.GroupTransactions = transactionsByDate;
            model.InitialDate = initialDate;
            model.EndDate = endDate;
            return model;
        }

        private (DateTime InitialDate, DateTime EndDate) InitialDateandEndGenerator(int month, int year)
        {
            DateTime initialDate;
            DateTime endDate;

            //Setting up initialDate and endDate
            if (month <= 0 || month > 12 || year <= 1900)
            {
                var today = DateTime.Today;
                initialDate = new DateTime(today.Year, today.Month, 1);
            }
            else
            {
                initialDate = new DateTime(year, month, 1);
            }

            endDate = initialDate.AddMonths(1).AddDays(-1);

            return (initialDate, endDate);
        }
    }
}
