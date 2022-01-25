using BudgetManagement.Interface;
using BudgetManagement.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BudgetManagement.Services
{
    public class CategoriesRepository : ICategoryRepository
    {
        private readonly string connectionString;

        public CategoriesRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Create(Category category)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>(
                @"Insert Into Categories (Name, OperationTypeId, UserId)
                Values (@Name, @OperationTypeId, @UserId);

                Select SCOPE_IDENTITY();", category);

            category.Id = id;
        }

        public async Task<IEnumerable<Category>> Get(int userId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Category>(
                "Select * from Categories Where UserId = @UserId", new { userId });
        }

        public async Task<IEnumerable<Category>> Get(int userId, OperationType operationTypeId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Category>(
                @"Select * from Categories 
                Where UserId = @UserId And OperationTypeId = @OperationTypeId", 
                new { userId, operationTypeId });
        }

        public async Task<Category> GetById(int id, int userId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Category>(
                @"Select * From Categories Where Id = @Id And UserId = @UserId",
                new { id, userId});
        }

        public async Task Update(Category category)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"Update Categories
                    Set Name = @Name, OperationTypeId = @OperationTypeId
                    Where Id = @Id", category);
        }

        public async Task Delete(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(
                @"Delete Categories Where Id = @Id", new { id });
        }
    }
}
