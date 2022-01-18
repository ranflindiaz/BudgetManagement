using BudgetManagement.Interface;
using BudgetManagement.Models;
using Microsoft.AspNetCore.Mvc;

namespace BudgetManagement.Controllers
{
    public class AccountTypesController : Controller
    {
        private readonly IAccountTypeRepository _accountTypeRepository;
        private readonly IUsersServices _usersServices;

        public AccountTypesController(IAccountTypeRepository accountTypeRepository, IUsersServices usersServices)
        {
            _accountTypeRepository = accountTypeRepository;
            _usersServices = usersServices;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _usersServices.GetUserId();
            var accountType = await _accountTypeRepository.GetAccountType(userId);
            return View(accountType);
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var userId = _usersServices.GetUserId();
            var accountType = await _accountTypeRepository.GetById(id, userId);

            if (accountType is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            return View(accountType);
        }

        [HttpPost]
        public async Task<IActionResult> Update(AccountType accountType)
        {
            var userId = _usersServices.GetUserId();
            var accountTypeExist = await _accountTypeRepository.GetById(accountType.Id, userId);

            if (accountTypeExist is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            await _accountTypeRepository.Update(accountType);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(AccountType accountType)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            accountType.UserId = _usersServices.GetUserId();

            var alreadyExist =
                await _accountTypeRepository.Exist(accountType.Name, accountType.UserId);

            if (alreadyExist)
            {
                ModelState.AddModelError(nameof(accountType.Name),
                    $"El nombre {accountType.Name} ya existe");

                return View(accountType);
            }

            await _accountTypeRepository.Create(accountType);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var userId = _usersServices.GetUserId();
            var accountType = await _accountTypeRepository.GetById(id, userId);

            if (accountType is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            return View(accountType);
        }

        [HttpPost]
        public async Task<IActionResult> DeleleAccountType(int id)
        {
            var userId = _usersServices.GetUserId();
            var accountType = await _accountTypeRepository.GetById(id, userId);

            if (accountType is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            await _accountTypeRepository.Delete(id);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> VerifyExistAccountType(string name)
        {
            var usuarioId = _usersServices.GetUserId();
            var alreadyExistAccountType = await _accountTypeRepository.Exist(name, usuarioId);

            if (alreadyExistAccountType)
            {
                return Json($"El nombre {name} ya existe");
            }

            return Json(true);
        }

        [HttpPost]
        public async Task<IActionResult> Organize([FromBody] int[] ids)
        {
            var userId = _usersServices.GetUserId();
            var accountTypes = await _accountTypeRepository.GetAccountType(userId);
            var idsAccountsTypes = accountTypes.Select(x => x.Id);

            var idsAccountsTypesUserDoesNotHave = ids.Except(idsAccountsTypes).ToList();

            if (idsAccountsTypesUserDoesNotHave.Count > 0)
            {
                return Forbid();
            }

            var accountTypesOrganize = ids.Select((valor, indice) =>
                new AccountType() { Id = valor, Orden = indice + 1 }).AsEnumerable();

            await _accountTypeRepository.Organize(accountTypesOrganize);

            return Ok();
        }
    }
}
