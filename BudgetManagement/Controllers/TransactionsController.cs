using AutoMapper;
using BudgetManagement.Interface;
using BudgetManagement.Models;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;

namespace BudgetManagement.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly IUsersServices _usersServices;
        private readonly IAccountsRepository _accountsRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ITransactionsRepository _transactionsRepository;
        private readonly IMapper _mapper;
        private readonly IReportsService _reportsService;

        public TransactionsController(IUsersServices usersServices, IAccountsRepository accountsRepository, ICategoryRepository categoryRepository,
            ITransactionsRepository transactionsRepository,
            IMapper mapper, IReportsService reportsService)
        {
            _usersServices = usersServices;
            _accountsRepository = accountsRepository;
            _categoryRepository = categoryRepository;
            _transactionsRepository = transactionsRepository;
            _mapper = mapper;
            _reportsService = reportsService;
        }

        public async Task<IActionResult> Index(int month, int year)
        {
            var userId = _usersServices.GetUserId();

            var model = await _reportsService.GetTransactionsDetailedReport(userId, month, year, ViewBag);

            return View(model);
        }

        public async Task<IActionResult> Weekly(int month, int year)
        {
            var userId = _usersServices.GetUserId();
            IEnumerable<GetByWeekResult> transactionByWeek = 
                await _reportsService.GetByWeek(userId, month, year, ViewBag);

            var grouped = transactionByWeek.GroupBy(x => x.Week).Select(x => new GetByWeekResult()
                {
                    Week = x.Key,
                    Income = x.Where(x => x.OperationTypeId ==
                    OperationType.Income).Select(x => x.Amount).FirstOrDefault(),
                    Spending = x.Where(x => x.OperationTypeId ==
                    OperationType.Spending).Select(x => x.Amount).FirstOrDefault()
                }).ToList();

            if (year == 0 || month ==0)
            {
                var today = DateTime.Today;
                year = today.Year;
                month = today.Month;
            }

            var referenceDate  = new DateTime(year, month, 1);
            var dayOfTheMonth = Enumerable.Range(1, referenceDate.AddMonths(1).AddDays(-1).Day);

            var segementedDays = dayOfTheMonth.Chunk(7).ToList();

            //Iterating segementedDays and creating weekdays by week
            for (int i = 0; i < segementedDays.Count(); i++)
            {
                var week = i + 1;
                var initialDate = new DateTime(year, month, segementedDays[i].First());
                var endDate = new DateTime(year, month, segementedDays[i].Last());
                var weekGroup = grouped.FirstOrDefault(x => x.Week == week);

                if (weekGroup is null)
                {
                    grouped.Add(new GetByWeekResult()
                    {
                        Week = week,
                        InitialDate = initialDate,
                        EndDate = endDate
                    });
                }
                else
                {
                    weekGroup.InitialDate = initialDate;
                    weekGroup.EndDate = endDate;
                }
            }

            grouped = grouped.OrderByDescending(x => x.Week).ToList();

            var model = new WeeklyReportViewModel();
            model.WeeklyTransactions = grouped;
            model.ReferenceDate = referenceDate;

            return View(model);
        }

        public async Task<IActionResult> Monthly(int year)
        {
            var userId = _usersServices.GetUserId();

            if (year == 0)
            {
                year = DateTime.Today.Year;
            }

            var transactionsByMonth = await _transactionsRepository.GetByMonth(userId, year);

            var groupedTransactions = transactionsByMonth.GroupBy(x => x.Month).Select(x => new GetByMonthResult()
            {
                Month = x.Key,
                Income = x.Where(x => x.OperationTypeId == OperationType.Income).Select(x => x.Amount).FirstOrDefault(),
                Spending = x.Where(x => x.OperationTypeId == OperationType.Spending).Select(x => x.Amount).FirstOrDefault()
            }).ToList();

            for (int month = 1; month <= 12; month++)
            {
                var transaction = groupedTransactions.FirstOrDefault(x => x.Month == month);
                var referenceDate = new DateTime(year, month, 1);
                if (transaction is null)
                {
                    groupedTransactions.Add(new GetByMonthResult()
                    {
                        Month = month,
                        ReferenceDate = referenceDate
                    });
                }
                else
                {
                    transaction.ReferenceDate = referenceDate;
                }
            }

            groupedTransactions = groupedTransactions.OrderByDescending(x => x.Month).ToList();

            var model = new MonthlyReportViewModel();
            model.Year = year;
            model.MonthlyTransactions = groupedTransactions;

            return View(model);
        }
        public IActionResult ReportExcel()
        {
            return View();
        }

        public async Task<FileResult> ExportExcelByMonth(int month, int year)
        {
            var initialDate = new DateTime(year, month, 1);
            var endDate = initialDate.AddMonths(1).AddDays(-1);
            var userId = _usersServices.GetUserId();

            var transactions = await _transactionsRepository.GetByUserId(new GetTransactionsParameterByUser
            {
                UserId = userId,
                InitialDate = initialDate,
                EndDate = endDate
            });

            var fileName = $"Budget Management - {initialDate.ToString("MMM yyyy")}.xlsx";

            return ExcelGenerator(fileName, transactions);

        }

        public async Task<FileResult> ExportExcelByYear(int year)
        {
            var initialDate = new DateTime(year, 1, 1);
            var endDate = initialDate.AddYears(1).AddDays(-1);
            var userId = _usersServices.GetUserId();

            var transactions = await _transactionsRepository.GetByUserId(new GetTransactionsParameterByUser
            {
                UserId = userId,
                InitialDate = initialDate,
                EndDate = endDate
            });

            var fileName = $"Budget Management - {initialDate.ToString("yyyy")}.xlsx";

            return ExcelGenerator(fileName, transactions);
        }

        public async Task<FileResult> ExportEverthingOnExcel()
        {
            var initialDate = DateTime.Today.AddYears(-100);
            var endDate = DateTime.Today.AddYears(1000);
            var userId = _usersServices.GetUserId();

            var transactions = await _transactionsRepository.GetByUserId(new GetTransactionsParameterByUser
            {
                UserId= userId,
                InitialDate= initialDate,
                EndDate= endDate
            });

            var fileName = $"Budget Managment - {DateTime.Today.ToString("MM-dd-yyyy")}.xlsx";

            return ExcelGenerator(fileName, transactions);
        }

        private FileResult ExcelGenerator(string fileName, IEnumerable<Transaction> transactions)
        {
            DataTable datatable = new DataTable("Transactions");
            datatable.Columns.AddRange(new DataColumn[] { 
              new DataColumn("Date"),
              new DataColumn("Account"),
              new DataColumn("Category"),
              new DataColumn("Note"),
              new DataColumn("Amount"),
              new DataColumn("Income/Spending")
            });

            foreach (var transaction in transactions)
            {
                datatable.Rows.Add(transaction.TransactionDate,
                    transaction.Account, transaction.Category,
                    transaction.Note, transaction.Amount,
                    transaction.OperationTypeId);
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(datatable);

                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), 
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }

        }

        public IActionResult Calendar()
        {
            return View();
        }

        public async Task<JsonResult> GetCalendarTransactions(DateTime start, DateTime end)
        {
            var userId = _usersServices.GetUserId();

            var transactions = await _transactionsRepository.GetByUserId(new GetTransactionsParameterByUser
            {
                UserId = userId,
                InitialDate = start,
                EndDate = end
            });

            var calendarEvents = transactions.Select(transaction => new CalendarEvent()
            {
                Title = transaction.Amount.ToString("N"),
                Start = transaction.TransactionDate.ToString("yyyy-MM-dd"),
                End = transaction.TransactionDate.ToString("yyyy-MM-dd"),
                Color = (transaction.OperationTypeId == OperationType.Spending) ? "Red" : null
            });

            return Json(calendarEvents);
        }

        public async Task<JsonResult> GetTransactionsByDate(DateTime date)
        {
            var userId = _usersServices.GetUserId();

            var transactions = await _transactionsRepository.GetByUserId(new GetTransactionsParameterByUser
            {
                UserId = userId,
                InitialDate = date,
                EndDate = date
            });

            return Json(transactions);
        }

        public async Task<IActionResult> Create()
        {
            var userId = _usersServices.GetUserId();
            var model = new CreateTransactionViewModel();
            model.Accounts = await GetAccounts(userId);
            model.Categories = await GetCategories(userId, model.OperationTypeId);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTransactionViewModel model)
        {
            var userId = _usersServices.GetUserId();

            if (!ModelState.IsValid)
            {
                model.Accounts = await GetAccounts(userId);
                model.Categories = await GetCategories(userId, model.OperationTypeId);
                return View(model);
            }

            var account = await _accountsRepository.GetById(model.AccountId, userId);
            if (account == null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            var categories = _categoryRepository.GetById(model.CategoryId, userId);
            if (categories is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            model.UserId = userId;

            if (model.OperationTypeId == OperationType.Spending)
            {
                model.Amount *= -1;
            }

            await _transactionsRepository.Create(model);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, string returnUrl = null)
        {
            var userId = _usersServices.GetUserId();

            var transaction = await _transactionsRepository.GetById(id, userId);

            if (transaction is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            await _transactionsRepository.Delete(id);
            if (string.IsNullOrEmpty(returnUrl))
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return LocalRedirect(returnUrl);
            }
        }

        //Get accounts base on userId
        private async Task<IEnumerable<SelectListItem>> GetAccounts(int userId)
        {
            var accounts = await _accountsRepository.Find(userId);
            return accounts.Select(x => new SelectListItem(x.Name, x.Id.ToString()));
        }

        //Get categories base on userId and operation type
        private async Task<IEnumerable<SelectListItem>> GetCategories(int userId, OperationType operationType)
        {
            var categories = await _categoryRepository.Get(userId, operationType);
            return categories.Select(x => new SelectListItem(x.Name, x.Id.ToString()));
        }

        [HttpPost]
        public async Task<IActionResult> GetCategories([FromBody] OperationType operationType)
        {
            var userId = _usersServices.GetUserId();
            var categories = await GetCategories(userId, operationType);
            return Ok(categories);
        }

        public async Task<IActionResult> Edit(int id, string returnUrl = null)
        {
            var userId = _usersServices.GetUserId();
            var transaction =await _transactionsRepository.GetById(id, userId);

            if (transaction == null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            var model = _mapper.Map<UpdateTransactionViewModel>(transaction);

            model.PreviousAmount = model.Amount;

            if (model.OperationTypeId == OperationType.Spending)
            {
                model.PreviousAmount = model.Amount * -1;
            }

            model.PreviousAccountId = transaction.AccountId;
            model.Categories = await GetCategories(userId, transaction.OperationTypeId);
            model.Accounts = await GetAccounts(userId);
            model.ReturnUrl = returnUrl;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateTransactionViewModel model)
        {
            var userId = _usersServices.GetUserId();

            if (!ModelState.IsValid)
            {
                model.Accounts = await GetAccounts(userId);
                model.Categories = await GetCategories(userId, model.OperationTypeId);
                return View(model);
            }

            var account = await _accountsRepository.GetById(model.AccountId, userId);
            if (account is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            var categories = await _categoryRepository.GetById(model.CategoryId, userId);
            if (categories is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            var transaction = _mapper.Map<Transaction>(model);

            if (transaction.OperationTypeId == OperationType.Spending)
            {
                transaction.Amount *= -1;
            }

            await _transactionsRepository.Update(transaction, model.PreviousAmount,
                model.PreviousAccountId);

            if (string.IsNullOrEmpty(model.ReturnUrl))
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return LocalRedirect(model.ReturnUrl);
            }
        }
    }
}
