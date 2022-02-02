using BudgetManagement.Interface;
using BudgetManagement.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;


namespace BudgetManagement.Services
{
    public class TransactionsRepository : ITransactionsRepository
    {
        private readonly string connectionString;

        public TransactionsRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Create (Transaction transaction)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>("Insert_Transaction", new { transaction.UserId, 
                transaction.TransactionDate, 
                transaction.Amount,
                transaction.CategoryId, 
                transaction.AccountId, 
                transaction.Note
              },
              commandType: System.Data.CommandType.StoredProcedure);

            transaction.Id = id;
        }

        public async Task<IEnumerable<Transaction>> GetByAccoundId(GetTransactionsByAccount model)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Transaction>
                (@"Select t.Id, t.Amount, t.TransactionDate, c.Name as Category,
                ac.Name as Account, c.OperationTypeId
                From Transactions t
                Inner Join Categories c
                On c.Id = t.CategoryId
                Inner join Accounts ac
                On ac.Id = t.AccountId
                Where t.AccountId = @AccountId And t.UserId = @UserId 
                And TransactionDate Between @InitialDate And @EndDate", model);
        }

        //get transactions by day and by userId
        public async Task<IEnumerable<Transaction>> GetByUserId(GetTransactionsParameterByUser model)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Transaction>
                (@"Select t.Id, t.Amount, t.TransactionDate, c.Name as Category,
                ac.Name as Account, c.OperationTypeId
                From Transactions t
                Inner Join Categories c
                On c.Id = t.CategoryId
                Inner join Accounts ac
                On ac.Id = t.AccountId
                Where t.UserId = @UserId 
                And TransactionDate Between @InitialDate And @EndDate
                Order By t.TransactionDate DESC", model);
        }

        public async Task Update(Transaction transaction, decimal previousAmount,
            int previousAccountId)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("Update_Transaction",
                new 
                { 
                    transaction.Id,
                    transaction.TransactionDate,
                    transaction.Amount,
                    transaction.CategoryId,
                    transaction.AccountId,
                    transaction.Note,
                    previousAmount,
                    previousAccountId
                }, commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<Transaction> GetById(int id, int userId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Transaction>(
                @"Select Transactions.*, cat.OperationTypeId
                From Transactions
                Inner Join Categories cat
                On cat.Id = Transactions.CategoryId
                Where Transactions.Id = @Id And Transactions.UserId = @UserId",
                new { id, userId});
        }

        public async Task<IEnumerable<GetByWeekResult>> GetByWeek(GetTransactionsParameterByUser model)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<GetByWeekResult>(
                @"Select DATEDIFF(D, @initialDate, TransactionDate) / 7 + 1 as Week,
                SUM(Amount) as Amount, cat.OperationTypeId
                From Transactions
                Inner Join Categories cat
                On cat.Id = Transactions.CategoryId
                Where Transactions.UserId = @userId And
                TransactionDate Between @initialDate And @endDate
                Group By DATEDIFF(D, @initialDate, TransactionDate) / 7, cat.OperationTypeId", model);
        }

        public async Task<IEnumerable<GetByMonthResult>> GetByMonth(int userId, int year)
        {
            var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<GetByMonthResult>(
                @"Select MONTH(TransactionDate) as month,
                Sum(Amount) as Amount, cat.OperationTypeId
                From Transactions
                Inner Join Categories cat
                On cat.Id = Transactions.CategoryId
                wHERE Transactions.UserId = @userId And Year(TransactionDate) = @year
                Group By Month(TransactionDate), cat.OperationTypeId", 
                new { userId, year });
        }

        public async Task Delete(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("Delete_Transactions",
                new { id }, commandType: System.Data.CommandType.StoredProcedure);
        }
    }
}
