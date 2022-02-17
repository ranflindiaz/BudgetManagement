using BudgetManagement.Models;

namespace BudgetManagement.Interface
{
    public interface IUsersRepository
    {
        Task<User> FindUserByEmail(string emailNormalized);
        Task<int> UserCreate(User user);
    }
}
