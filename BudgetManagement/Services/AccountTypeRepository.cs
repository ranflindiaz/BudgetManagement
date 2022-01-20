using BudgetManagement.Interface;
using BudgetManagement.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BudgetManagement.Services
{
    public class AccountTypeRepository : IAccountTypeRepository
    {
        private readonly string connectionString;
        public AccountTypeRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<AccountType>> GetAccountType(int userId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<AccountType>(
                @"SELECT Id, Name, Orden From AccountsTypes WHERE UserId = @UserId ORDER BY Orden", new { userId });
        }

        public async Task Create(AccountType accountType)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>(
                "Insert_AccountsTypes",
                new
                {
                    UserId = accountType.UserId,
                    Name = accountType.Name
                },
                commandType: System.Data.CommandType.StoredProcedure);

            accountType.Id = id;
        }

        public async Task<bool> Exist(string name, int userId)
        {
            using var connection = new SqlConnection(connectionString);
            var exist = await connection.QueryFirstOrDefaultAsync<int>
                (@"SELECT 1 FROM AccountsTypes WHERE Name = @Name AND UserId = @UserId;",
                new { name, userId });

            return exist == 1;
        }

        public async Task Update(AccountType accountType)
        {
            var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(
                "Update AccountsTypes " +
                "Set Name = @Name Where Id = @Id", accountType);
        }

        public async Task<AccountType> GetById(int id, int userId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<AccountType>(
                @"Select Id, Name, Orden 
                From AccountsTypes 
                Where Id = @Id AND UserId = @UserId",
                new { id, userId });
        }

        public async Task Delete(int id)
        {
            var connection = new SqlConnection(connectionString);

            await connection.ExecuteAsync("Delete AccountsTypes where Id = @Id", new { id });
        }

        public async Task Organize(IEnumerable<AccountType> accountTypes)
        {
            var query = "Update AccountsTypes Set Orden = @Orden Where Id = @Id";

            using var connection = new SqlConnection(connectionString);

            await connection.ExecuteAsync(query, accountTypes);
        }
    }
}
