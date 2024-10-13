using System;
namespace Rocket.Core
{
    [Serializable]
    public class Transaction
    {
        public Transaction(){}
        public Transaction(TransactionSource source,TransactionType type,double amount,string description = ""){
            Source = source;
            Type = type;
            Amount = amount;
            Description = description;
        }
        public TransactionSource Source { get; set; }
        public TransactionType Type { get; set; }
        public double Amount { get; set; }
        public string Description { get; set; }
    }
}