namespace BudgetManagement.Models
{
    public class DetailedTransactionsReport
    {
        public DateTime InitialDate { get; set; }

        public DateTime EndDate { get; set; }

        public IEnumerable<TransactionsByDate> GroupTransactions { get; set; }

        public decimal DepositBalances => GroupTransactions.Sum(x => x.DepositBalances);

        public decimal WithdrawBalances => GroupTransactions.Sum(x => x.WithdrawBalances);

        public decimal Total => DepositBalances - WithdrawBalances;

        public class TransactionsByDate
        {
            public DateTime TransactionDate { get; set; }

            public IEnumerable<Transaction> Transactions { get; set;}

            public decimal DepositBalances => 
                Transactions.Where(x => x.OperationTypeId == OperationType.Income)
                .Sum(x => x.Amount);

            public decimal WithdrawBalances =>
                Transactions.Where(x => x.OperationTypeId == OperationType.Spending)
                .Sum(x => x.Amount);
        }
    }
}
