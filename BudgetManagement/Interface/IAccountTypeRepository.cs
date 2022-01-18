using BudgetManagement.Models;

namespace BudgetManagement.Interface
{
    public interface IAccountTypeRepository
    {
        Task Create(AccountType accountType);

        Task<bool> Exist(string name, int userId);

        Task<IEnumerable<AccountType>> GetAccountType(int userId);

        Task<AccountType> GetById(int id, int userId);

        Task Update(AccountType accountType);

        Task Delete(int id);

        Task Organize(IEnumerable<AccountType> accountTypes);
    }
}
