using AutoMapper;
using BudgetManagement.Interface;
using BudgetManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BudgetManagement.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly IUsersServices _usersServices;
        private readonly IAccountsRepository _accountsRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ITransactionsRepository _transactionsRepository;
        private readonly IMapper _mapper;

        public TransactionsController(IUsersServices usersServices, IAccountsRepository accountsRepository, ICategoryRepository categoryRepository,
            ITransactionsRepository transactionsRepository,
            IMapper mapper)
        {
            _usersServices = usersServices;
            _accountsRepository = accountsRepository;
            _categoryRepository = categoryRepository;
            _transactionsRepository = transactionsRepository;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
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
