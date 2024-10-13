using System;
using System.Collections.Generic;

namespace Rocket.Core
{
    public interface IHasTransactions
    {
        List<Transaction> Transactions { get; set; }
        double Balance(TransactionSource? source);
        double Debit(TransactionSource source);
        double Transit(TransactionSource? source);
        double Transit(TransactionSource? source, Func<string, bool> filter);
    }
}