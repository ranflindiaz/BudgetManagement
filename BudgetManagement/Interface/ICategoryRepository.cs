using BudgetManagement.Models;

namespace BudgetManagement.Interface
{
    public interface ICategoryRepository
    {
        Task Create(Category category);

        Task Delete(int id);

        Task<IEnumerable<Category>> Get(int userId);

        Task<IEnumerable<Category>> Get(int userId, OperationType operationTypeId);

        Task<Category> GetById(int id, int userId);

        Task Update(Category category);
    }
}
