using BudgetManagement.Interface;
using BudgetManagement.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BudgetManagement.Services
{
    public class UsersRepository : IUsersRepository
    {
        private readonly string connectionString;
        public UsersRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<int> UserCreate(User user)
        {
            using var connection = new SqlConnection(connectionString);
            var userId = await connection.QuerySingleAsync<int>(
                @"Insert Into Users (Email, NormalizedEmail, PasswordHash)
                Values (@Email, @NormalizedEmail, @PasswordHash);
                Select SCOPE_IDENTITY();
                ", user);

            await connection.ExecuteAsync("NewUserDataCreate", new { userId },
                commandType: System.Data.CommandType.StoredProcedure);

            return userId;
        }

        public async Task<User> FindUserByEmail(string normalizedEmail)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QuerySingleOrDefaultAsync<User>(
                @"Select * from Users Where NormalizedEmail = @normalizedEmail", new { normalizedEmail });
        }
    }
}
