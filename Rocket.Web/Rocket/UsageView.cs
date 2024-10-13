using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.Core;
using Rocket.States;

namespace Rocket
{
    [Serializable]
    public class UsageView
    {
        public UsageView()
        {
            Transactions = new List<Transaction>();
        }

        public List<Transaction> Transactions { get; set; }

        public double Balance(params TransactionSource[] sources)
        {
            double value = 0.0;
            IEnumerable<Transaction> targets = Transactions;
            if (sources != null)
            {
                targets = Transactions.Where((item) => sources.Contains(item.Source));
            }

            foreach (Transaction item in targets)
            {
                if(double.IsNaN(item.Amount))
                    item.Amount = 0.0;
                value += ((item.Type == TransactionType.Debit) ? item.Amount : (item.Amount * -1));
            }
            if (value < 0)
                value = 0;
            return Math.Round(value,4);
        }

        public double ExtraUsage { get; set; }
        public double MonthlyAverage { get; set; }
        public double Consumption { get; set; }
        public double Tariff { get; set; }
        public long EstateId { get; set; }
        public double SmallAmountThisMonth { get; set; }
        public double DepreciationCredit { get; set; }
        public DateTime ReadDateNow { get; set; }
        public int Duration { get; set; }
        public string Period { get; set; }

        public double Credit
        {
            get
            {
                if (Tariff == 5)
                    return 0;
                return Math.Round(Transactions.Where(item => item.Type == TransactionType.Credit).Sum(record => double.IsNaN(record.Amount) ? 0 : record.Amount), 4);
            }
        }

        public double Discount
        {
            get
            {
                return Math.Round(Transactions.Where(item => item.Type == TransactionType.Discount).Sum(record => double.IsNaN(record.Amount) ? 0 : record.Amount), 4);
            }
        }

        public double Debit
        {
            get
            {
                return Math.Round(Transactions.Where(item => item.Type == TransactionType.Debit).Sum(record => double.IsNaN(record.Amount) ? 0 : record.Amount), 4);
            }
        }

        public double Payable
        {
            get
            {
                double normalize = (Debit - Discount) <= 0 ? 0 : (Debit - Discount);
                double value = normalize - Credit;
                SmallAmountThisMonth = 0;
                if (value < 0)
                {
                    DepreciationCredit = normalize;
                    return 0;
                }
                DepreciationCredit = Credit;
                SmallAmountThisMonth = (value % 1000);
                if (SmallAmountThisMonth < 1000)
                {
                    value -= SmallAmountThisMonth;
                }
                return Math.Round(value,4);
            }
        }

    }
}