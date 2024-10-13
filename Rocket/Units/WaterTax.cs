using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.Core;
using Rocket.Options;

namespace Rocket.Units
{
    public class WaterTax: Unit, IObserver
    {
        public WaterTax(TimeSeries series, TaxOptions options, int? illegalMode = null)
            : base()
        {
            Options = options;
            DataSeries = series;
            IllegalMode = illegalMode;
        }
        public TaxOptions Options { get; set; }
        public TimeSeries DataSeries { get; set; }
        public int? IllegalMode { get; set; }

        public override void Update(object sender, EventArgs e)
        {
            Reset(TransactionSource.WaterTax);
            IUsageSubject usage = Subject as IUsageSubject;
            if (Options.Karbari == 1 && Options.IsVillage && Options.CategoryUses == 154)
                return;
            if (Options.CategoryUses == 70)
                return;

            
            double price = 0;
            double rate_per_day = 0;
            int[] categories = { 55, 79, 262, 97, 238, 271 };
            IHasTransactions ts = Subject as IHasTransactions;
            if (IllegalMode != null && IllegalMode.Value == 10)
            {
                ts.Transactions.RemoveAll((item) =>
                {
                    return new TransactionSource[] { TransactionSource.WasteWater, TransactionSource.SubscriptionSewage }.Contains(item.Source) == false;
                    //return item.Source != TransactionSource.WasteWater && item.Source != TransactionSource.SubscriptionSewage;
                });
            }
            if (categories.Contains(Options.CategoryUses))
                rate_per_day = (ts.Balance(TransactionSource.WaterCost) + ts.Balance(TransactionSource.FreeWater) + ts.Balance(TransactionSource.WaterPattern) + ts.Balance(TransactionSource.Seasonal)) / usage.Duration.Days.ToDouble();
            else
                rate_per_day = (ts.Balance(TransactionSource.WaterCost) + ts.Balance(TransactionSource.FreeWater) + ts.Balance(TransactionSource.WaterPattern) + ts.Balance(TransactionSource.Subscription) + ts.Balance(TransactionSource.Seasonal) + ts.Balance(TransactionSource.Made7)) / usage.Duration.Days.ToDouble();
            InTime timeObj = usage.InTime();
            Frame<Block> Series = new Frame<Block>(EntryHash.Empty);
            Frame<Block> SRHash = DataSeries.Select(new EntryHash(EntryType.Water, Options.CityId, Options.Tariff, 19, 0, 0, 1));
            //Entry SRRate = D[SRHash].Slice(Options,false);
            LinkedList<Dictionary<string, Block>> rates = Series.Transform(new MergeFrame(SRHash))
                                                                .Extract<LinkedList<Dictionary<string, Block>>>(new Link(timeObj.From, timeObj.To,usage));
            LinkedListNode<Dictionary<string, Block>> currentStep = rates.First;
            int maxLoop = 10;
            while (currentStep != null)
            {
                double rangeDuration = currentStep.Value.First().Value.Duration.Days.ToString().ToDouble();
                price += rate_per_day * rangeDuration * currentStep.Value["19"].Value;
                currentStep = currentStep.Next;
                maxLoop--;
                if (maxLoop == 0)
                    break;
            }
            Effect(new Transaction(TransactionSource.WaterTax,TransactionType.Debit,price));
            if (categories.Contains(Options.CategoryUses))
                Effect(new Transaction(TransactionSource.WaterTax, TransactionType.Discount, price));
        }
        
    }
}