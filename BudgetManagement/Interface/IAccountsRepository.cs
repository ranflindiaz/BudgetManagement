using BudgetManagement.Models;

namespace BudgetManagement.Interface
{
    public interface IAccountsRepository
    {
        Task Create(Account account);

        Task<IEnumerable<Account>> Find(int userId);

        Task<Account> GetById(int id, int userId);

        Task Update(CreateAccountViewModel account);
    }
}
