using BudgetManagement.Interface;
using BudgetManagement.Models;
using Microsoft.AspNetCore.Mvc;

namespace BudgetManagement.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUsersServices _usersServices;

        public CategoriesController(ICategoryRepository categoryRepository,
            IUsersServices usersServices)
        {
            _categoryRepository = categoryRepository;
            _usersServices = usersServices;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _usersServices.GetUserId();
            var category = await _categoryRepository.Get(userId);
            return View(category);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View(category);
            }

            var userId = _usersServices.GetUserId();
            category.UserId = userId;
            await _categoryRepository.Create(category);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var userId = _usersServices.GetUserId();
            var category = await _categoryRepository.GetById(id, userId);

            if (category is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Category categoryEdit)
        {

            if (!ModelState.IsValid)
            {
                return View(categoryEdit);
            }

            var userId = _usersServices.GetUserId();
            var category = await _categoryRepository.GetById(categoryEdit.Id, userId);

            if (category is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            categoryEdit.UserId = userId;

            await _categoryRepository.Update(categoryEdit);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var userId = _usersServices.GetUserId();
            var category = await _categoryRepository.GetById(id, userId);

            if (category is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var userId = _usersServices.GetUserId();
            var category = await _categoryRepository.GetById(id, userId);

            if (category is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            await _categoryRepository.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
