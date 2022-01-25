using BudgetManagement.Models;

namespace BudgetManagement.Interface
{
    public interface IAccountsRepository
    {
        Task Create(Account account);
        Task Delete(int id);
        Task<IEnumerable<Account>> Find(int userId);

        Task<Account> GetById(int id, int userId);

        Task Update(CreateAccountViewModel account);
    }
}
