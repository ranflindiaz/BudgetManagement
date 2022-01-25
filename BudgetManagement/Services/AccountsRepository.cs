using BudgetManagement.Interface;
using BudgetManagement.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BudgetManagement.Services
{
    public class AccountsRepository : IAccountsRepository
    {
        private readonly string connectionString;

        public AccountsRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Create(Account account)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>(
                @"Insert INTO Accounts (Name, AccountTypeId, Balance, Description)
                Values (@Name, @AccountTypeId, @Balance, @Description);

                Select SCOPE_IDENTITY();", account);

            account.Id = id;
        }

        public async Task<IEnumerable<Account>> Find(int userId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Account>(
                @"Select Accounts.Id, Accounts.Name, Balance, tc.Name as AccountType
                From Accounts
                Inner join AccountsTypes tc
                On tc.Id = Accounts.AccountTypeId
                Where tc.UserId = @UserId
                Order By tc.Orden", new { userId });
        }

        public async Task<Account> GetById(int id, int userId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Account>(
                @"Select Accounts.Id, Accounts.Name, Balance, Description, tc.Id
                From Accounts
                Inner join AccountsTypes tc
                On tc.Id = Accounts.AccountTypeId
                Where tc.UserId = @UserId And Accounts.Id = @Id
                Order By tc.Orden", new { id, userId });
        }

        public async Task Update(CreateAccountViewModel account)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(
                @"Update Accounts
                Set Name = @Name, Balance = @Balance, Description = @Description,
                AccountTypeId = @AccountTypeId
                Where Id = @Id", account );
        }

        public async Task Delete(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("Delete Accounts Where Id = @Id", new { id });
        }
    }
}
