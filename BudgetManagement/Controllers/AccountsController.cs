using BudgetManagement.Interface;
using BudgetManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BudgetManagement.Controllers
{
    public class AccountsController : Controller
    {
        private readonly IAccountTypeRepository _accountsTypes;
        private readonly IUsersServices _usersServices;

        public AccountsController(IAccountTypeRepository accountsTypes,
            IUsersServices usersServices)
        {
            _accountsTypes = accountsTypes;
            _usersServices = usersServices;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var userId = _usersServices.GetUserId();
            var accountsTypes = await _accountsTypes.GetAccountType(userId);
            var model = new CreateAccountViewModel();
            model.AccountTypes = accountsTypes.Select(x => new SelectListItem(x.Name, x.Id.ToString()));

            return View(model);
        }
    }
}
