using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.Core;
using Rocket.Options;

namespace Rocket.Units
{
    public class BrokenPipe: Unit, IObserver
    {
        public BrokenPipe(TimeSeries series, BrokenPipeOptions options):base(){
            Options = options;
            DataSeries = series;
        }
        public BrokenPipeOptions Options { get; set; }
        public TimeSeries DataSeries { get; set; }

        public override void Update(object sender, EventArgs e)
        {
            Reset(TransactionSource.BrokenPipe);
            IUsageSubject usage = Subject as IUsageSubject;
            if (Options.Karbari == 1 && Options.IsVillage && Options.CategoryUses == 154)
                return;
            InTime timeObj = usage.InTime();
            // Just Broken Type Accepted
            if (Options.ReadCode != 5)
                return;
            // Tariff Must Be House Hold
            if (Options.Tariff != 11)
                return;
            double maxRate = this.MaxRule(usage);
            double max = (maxRate / 30.0) * double.Parse(usage.Duration.Days.ToString()) * Options.TedadVahed;
            double actual = (usage.Consumption) * Options.TedadVahed;
            double factor = (max < actual) ? max : actual;
            double amount = (usage.Consumption * Options.TedadVahed) - factor;

            IUsageSubject clonedUsage = usage.Clone(null,null,null,amount,null,null);
            Frame<Block> Series = new Frame<Block>(EntryHash.Empty);
            Frame<Block> BPHash = DataSeries.Select(new EntryHash(EntryType.Water, Options.CityId, 86, 17, 0, 0, 1)).Transform(new InVolume(factor / double.Parse(usage.Duration.Days.ToString()) * 30));
            Frame<Block> CHash = DataSeries.Select(new EntryHash(EntryType.Water, Options.CityId, 0, 14, 0, 0, 1));
            LinkedList<Dictionary<string, Block>> rates = Series.Transform(new MergeFrame(BPHash)).Extract<LinkedList<Dictionary<string, Block>>>(new Link(timeObj.From, timeObj.To, usage));
            
            LinkedListNode<Dictionary<string, Block>> currentStep = rates.First;
            double price = 0;
            int maxLoop1 = 10;
            ApplyCityRate applyCityRate = new ApplyCityRate(DataSeries);
            while (currentStep != null)
            {
                var currentUsage = currentStep.Value.First().Value.Usage;
                price += applyCityRate.Apply(currentUsage, Options.CityId, Options.Tariff, Options.IsVillage, 1, (r, cityDuration) =>
                {
                    return ((amount * cityDuration) / double.Parse(usage.Duration.Days.ToString())) * currentStep.Value["17"].Value * r;
                    //priceItem_v2 * patternUsage.Consumption * r * (cityDuration / formulaUsage.Duration.Days.ToDouble());
                });
                //double rangeDuration = double.Parse(currentStep.Value.First().Value.Duration.Days.ToString());
                //price += ((amount * rangeDuration) / double.Parse(usage.Duration.Days.ToString())) * currentStep.Value["17"].Value * currentStep.Value["14"].Value;
                currentStep = currentStep.Next;
                maxLoop1--;
                if (maxLoop1 == 0)
                    break;
            }
            if (Options.IsVillage && Options.Karbari == 1)
                price = price / 2;
            Effect(new Transaction(TransactionSource.BrokenPipe,TransactionType.Debit,price));
        }
        public double MaxRule(IUsageSubject usage){
            double maxRate = 0;
            InTime timeObj = usage.InTime(); 
            Frame<Block> MHash = DataSeries.Select(EntryHash.GetKey(EntryType.Water, Options.CityId, 86, 1, 0, 0, 1));
            if (MHash.Count() > 0)
            {

                LinkedList<Dictionary<string, Block>> blocks = MHash.Extract<LinkedList<Dictionary<string, Block>>>(new Link(timeObj.From, timeObj.To, usage));
                LinkedListNode<Dictionary<string, Block>> step = blocks.First;
                int maxLoop = 10;
                while (step != null)
                {
                    if (maxRate < step.Value["1"].Value)
                        maxRate = step.Value["1"].Value;
                    step = step.Next;
                    maxLoop--;
                    if (maxLoop == 0)
                        break;
                }
                
            }

            if(maxRate ==0){
                maxRate = 42.5;
            }
            return maxRate;
        }
        public double AmountByMaxRule(IUsageSubject usage)
        {
            double maxRate = this.MaxRule(usage);
            double max = (maxRate / 30.0) * double.Parse(usage.Duration.Days.ToString()) * Options.TedadVahed;
            double actual = (usage.Consumption) * Options.TedadVahed;
            return (max < actual) ? max : actual;
        }
        
    }
}