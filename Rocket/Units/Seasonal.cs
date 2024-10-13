using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.Core;
using Rocket.Options;

namespace Rocket.Units
{
    public class Seasonal: Unit, IObserver
    {
        public Seasonal(TimeSeries series, SeasonalOptions options):base(){
            Options = options;
            DataSeries = series;
        }
        public SeasonalOptions Options { get; set; }
        public TimeSeries DataSeries { get; set; }

        public override void Update(object sender, EventArgs e)
        {
            Reset(TransactionSource.Seasonal);
            IUsageSubject usage = Subject as IUsageSubject;

            if (Options.Karbari == 1 && Options.IsVillage && Options.CategoryUses == 154)
                return;
            int[] codes = { 66, 76 };
            int[] tariffs = { 7, 9 };
            if (Options.CategoryUses == 70 || (codes.Contains(Options.CityId) && tariffs.Contains(Options.Tariff)))
                return;
            bool type = (Options.Tariff == 11);
            InTime timeObj = usage.InTime();
            Frame<Block> Series = new Frame<Block>(EntryHash.Empty);
            Frame<Block> SHash = DataSeries.Select(new EntryHash(EntryType.Water, Options.CityId, Options.Tariff == 11?11:0, 20, 0, 0, 1)).Transform(new InVolume(usage.MonthlyAverage));
            //Entry SRate = D[SHash].Slice(options);
            LinkedList<Dictionary<string, Block>> rates = Series.Transform(new MergeFrame(SHash))
                    .Extract<LinkedList<Dictionary<string, Block>>>(new Link(timeObj.From, timeObj.To,usage));
            LinkedListNode<Dictionary<string, Block>> currentStep = rates.First;
            IHasTransactions ts = Subject as IHasTransactions;
            double rate = (ts.Balance(TransactionSource.WaterCost) + ts.Balance(TransactionSource.FreeWater) + ts.Balance(TransactionSource.WaterPattern) + ts.Balance(TransactionSource.BrokenPipe)) / double.Parse(usage.Duration.Days.ToString());
            double price = 0;
            int maxLoop = 10;
            while (currentStep != null)
            {
                double rangeDuration = currentStep.Value.First().Value.Duration.Days.ToString().ToDouble();
                price += (rate * rangeDuration * currentStep.Value["20"].Value);
                currentStep = currentStep.Next;
                maxLoop--;
                if (maxLoop == 0)
                    break;
            }
            Effect(new Transaction(TransactionSource.Seasonal,TransactionType.Debit,price));
            int[] categories = { 55, 79, 262, 97, 238, 271 };
            if (categories.Contains(Options.CategoryUses))
            {
                Effect(new Transaction(TransactionSource.Seasonal,TransactionType.Discount,price));
            }
        }
        
    }
}