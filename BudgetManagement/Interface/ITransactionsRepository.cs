using BudgetManagement.Models;

namespace BudgetManagement.Interface
{
    public interface ITransactionsRepository
    {
        Task Create(Transaction transaction);
        Task Delete(int id);
        Task<IEnumerable<Transaction>> GetByAccoundId(GetTransactionsByAccount model);
        Task<Transaction> GetById(int id, int userId);
        Task Update(Transaction transaction, decimal previousAmount, int previousAccount);
    }
}
