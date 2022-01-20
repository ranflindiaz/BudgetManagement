﻿namespace BudgetManagement.Models
{
    public class AccountIndexViewModel
    {
        public string AccountType { get; set; }

        public IEnumerable<Account> Accounts { get; set; }

        public decimal Balance => Accounts.Sum(x => x.Balance);
    }
}
