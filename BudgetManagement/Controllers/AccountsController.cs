using AutoMapper;
using BudgetManagement.Interface;
using BudgetManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BudgetManagement.Controllers
{
    public class AccountsController : Controller
    {
        private readonly IAccountTypeRepository _accountsTypesRepository;
        private readonly IUsersServices _usersServices;
        private readonly IAccountsRepository _accountsRepository;
        private readonly IMapper _mapper;
        private readonly ITransactionsRepository _transactionsRepository;
        private readonly IReportsService _reportsService;

        public AccountsController(IAccountTypeRepository accountsTypesRepository,
            IUsersServices usersServices, IAccountsRepository accountsRepository,
            IMapper mapper, ITransactionsRepository transactionsRepository,
            IReportsService reportsService)
        {
            _accountsTypesRepository = accountsTypesRepository;
            _usersServices = usersServices;
            _accountsRepository = accountsRepository;
            _mapper = mapper;
            _transactionsRepository = transactionsRepository;
            _reportsService = reportsService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _usersServices.GetUserId();
            var accountsWithAccountType = await _accountsRepository.Find(userId);

            var model = accountsWithAccountType
                .GroupBy(x => x.AccountType)
                .Select(group => new AccountIndexViewModel
                {
                    AccountType = group.Key,
                    Accounts = group.AsEnumerable()
                }).ToList();

            return View(model);
        }

        //Get transactions detailed by month
        public async Task<IActionResult> Details(int id, int month, int year)
        {
            var userId = _usersServices.GetUserId();
            var account = await _accountsRepository.GetById(id, userId);

            if (account is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            ViewBag.Account = account.Name;

            var model = await _reportsService.
                GetDetailedTransactionsReportByAccount(
                userId, id, month, year, ViewBag);

            return View(model);
        }

        public async Task<IActionResult> Create()
        {
            var userId = _usersServices.GetUserId();
            var model = new CreateAccountViewModel();
            model.AccountTypes = await GetAccountTypes(userId);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateAccountViewModel account)
        {
            var userId = _usersServices.GetUserId();
            var accountType = await _accountsTypesRepository.GetById(account.AccountTypeId, userId);

            if (accountType is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            if (!ModelState.IsValid)
            {
                account.AccountTypes = await GetAccountTypes(userId);
                return View(account);
            }

            await _accountsRepository.Create(account);
            return RedirectToAction("Index");

        }

        public async Task<IActionResult> Edit(int id)
        {
            var userId = _usersServices.GetUserId();
            var account = await _accountsRepository.GetById(id, userId);

            if (account is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            var model = _mapper.Map<CreateAccountViewModel>(account);

            model.AccountTypes = await GetAccountTypes(userId);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CreateAccountViewModel accountEdit)
        {
            var userId = _usersServices.GetUserId();
            var account = await _accountsRepository.GetById(accountEdit.Id, userId);

            if (account is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            var accountType = await _accountsTypesRepository.GetById(accountEdit.AccountTypeId, userId);

            if (accountType is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            await _accountsRepository.Update(accountEdit);
            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> Delete(int id)
        {
            var userId = _usersServices.GetUserId();
            var account = await _accountsRepository.GetById(id, userId);

            if (account is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            return View(account);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            var userId = _usersServices.GetUserId();
            var account = await _accountsRepository.GetById(id, userId);

            if (account is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            await _accountsRepository.Delete(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task<IEnumerable<SelectListItem>> GetAccountTypes(int userId)
        {
            var accountsTypes = await _accountsTypesRepository.GetAccountType(userId);
            return accountsTypes.Select(x => new SelectListItem(x.Name, x.Id.ToString()));
        }


    }
}
