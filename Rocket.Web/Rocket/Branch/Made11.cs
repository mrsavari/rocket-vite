using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.Core;
using Rocket.States;
using Rocket.Options;

namespace Rocket.Branch
{
    public class Made11 : Unit, IObserver
    {
        public Made11(TimeSeries series, Made11Options options)
            : base()
        {
            Options = options;
            DataSeries = series;
        }
        public Made11Options Options { get; set; }
        public TimeSeries DataSeries { get; set; }

        public override void Update(object sender, EventArgs e)
        {
            TransactionSource ts = TransactionSource.Made11AB;
            if (Options.Type == ResourceType.Water)
            {
                Reset(TransactionSource.Made11AB);
                ts = TransactionSource.Made11AB;
            }
            else
            {
                Reset(TransactionSource.Made11FA);
                ts = TransactionSource.Made11FA;
            }
            string Description = Options.Type == ResourceType.Water ? "تخفیف آب" : "تخفیف فاضلاب";
            /*List<Transaction> selected = new List<Transaction>();
            selected.AddRange((Subject as IHasTransactions).Transactions.Where(item => item.Source == TransactionSource.Made7 && item.Description == Description));
            foreach (Transaction item in selected)
            {
                (Subject as IHasTransactions).Transactions.Remove(item);
            }*/
            IUsageSubject usage = Subject as IUsageSubject;
            if (Options.IsVillage)
                return;
            if (Options.Type == ResourceType.Branch && Options.KindBranch != 1)
                return;
            int costTypeId = 0;
            if (Options.Karbari == 1)
                costTypeId = 18;
            else
                costTypeId = 17;
            InTime timeObj = usage.InTime();

            double PriceFactor = 0;
            double baseCost = 0;
            if (Options.Tariff != 11)
            {
                Frame<Block> frames = DataSeries.Select(new EntryHash(EntryType.Branch, Options.CityId, 1, costTypeId, 0, 0, 1));
                if (Options.Tariff != 11)
                    PriceFactor = frames.Transform(new InVolume(usage.Capacity)).Extract<LinkedList<Dictionary<string, Block>>>(new Link(timeObj.From, timeObj.To, null)).First.Value[costTypeId.ToString()].Value;
                baseCost = PriceFactor * usage.Capacity;
            }
            else
            {
                List<Transaction> transactions = (Subject as IHasTransactions).Transactions;
                if (Options.Type == ResourceType.Water)
                    PriceFactor = transactions.Where(item => item.Source == TransactionSource.WaterMembershipFee && item.Type == TransactionType.Debit).Sum(rec => rec.Amount);
                else
                    PriceFactor = transactions.Where(item => item.Source == TransactionSource.SewageMembershipFee && item.Type == TransactionType.Debit).Sum(rec => rec.Amount);

                LinkedList<Dictionary<string, Block>> rates = DataSeries.Select(new EntryHash(EntryType.Branch, Options.CityId, 1, costTypeId, 0, 0, 0)).Extract<LinkedList<Dictionary<string, Block>>>(new Link(timeObj.From, timeObj.To, null));
                baseCost = PriceFactor * (rates.First.Value[costTypeId.ToString()].Value / 100);
                //New Change: BEGIN
                baseCost += rates.First.Value[costTypeId.ToString()].Extra;
                //New Change: END
            }
            if (baseCost <= 0)
                return;

            Effect(new Transaction(ts, TransactionType.Debit, baseCost));

            double DiscountFactor = Options.Type == ResourceType.Water ? Options.TakhfifENAB : Options.TakhfifENFA;
            if (DiscountFactor > 0 && this.Balance > 0)
            {
                if (DiscountFactor >= 100)
                {
                    Effect(new Transaction(ts, TransactionType.Discount, this.Balance, Description));
                }
                else
                {
                    Effect(new Transaction(ts, TransactionType.Discount, this.Balance * (DiscountFactor / 100), Description));
                }
            }

        }

    }
}