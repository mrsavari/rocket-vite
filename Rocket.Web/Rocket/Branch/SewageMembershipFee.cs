using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.Core;
using Rocket.States;
using Rocket.Options;

namespace Rocket.Branch
{
    public class SewageMembershipFee: Unit, IObserver
    {
        public SewageMembershipFee(TimeSeries series, SewageMembershipOptions options)
            : base()
        {
            Options = options;
            DataSeries = series;
        }
        public SewageMembershipOptions Options { get; set; }
        public TimeSeries DataSeries { get; set; }

        public override void Update(object sender, EventArgs e)
        {
            Reset(TransactionSource.SewageMembershipFee);
            IUsageSubject usage = Subject as IUsageSubject;
            if (Options.IsVillage || Options.KindBranch == 2 || new double[] { 7, 34, 33 }.Contains(Options.Karbari))
                return;

            List<Transaction> transactions = (Subject as IHasTransactions).Transactions;
            double PriceFactor = transactions.Where(item => item.Source == TransactionSource.WaterMembershipFee && item.Type == TransactionType.Debit).Sum(rec => rec.Amount);
            int tariff = Options.Karbari == 1 ? 11 : 0;
            if (Options.Karbari == 1)
            {
                InTime timeObj = usage.InTime();
                LinkedList<Dictionary<string, Block>> rates = DataSeries.Select(new EntryHash(EntryType.Branch, 0, tariff, 13, 0, 0, 1))
                                                               .Extract<LinkedList<Dictionary<string, Block>>>(new Link(timeObj.From, timeObj.To, null));
                if (rates.Count > 0)
                {
                    Effect(new Transaction(TransactionSource.SewageMembershipFee, TransactionType.Debit, PriceFactor * rates.First.Value["13"].Value));
                }
            }
            else
            {
                Effect(new Transaction(TransactionSource.SewageMembershipFee, TransactionType.Debit, PriceFactor));
            }

            double DiscountFactor =  Options.TakhfifENFA;
            if (DiscountFactor > 0 && this.Balance > 0)
            {
                if (DiscountFactor >= 100)
                {
                    Effect(new Transaction(TransactionSource.SewageMembershipFee, TransactionType.Discount, this.Balance));
                }
                else
                {
                    Effect(new Transaction(TransactionSource.SewageMembershipFee, TransactionType.Discount, this.Balance * (DiscountFactor / 100)));
                }
            }

        }
        
    }
}