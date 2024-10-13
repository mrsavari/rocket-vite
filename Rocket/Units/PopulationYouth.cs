using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.Core;
using Rocket.Options;

namespace Rocket.Units
{
    public class PopulationYouth: Unit, IObserver
    {
        public PopulationYouth(TimeSeries series, PopulationYouthOptions options, int? serialNo = null)
            : base()
        {
            Options = options;
            DataSeries = series;
            SerialNo = serialNo;
        }

        public int? SerialNo { get; set; }
        public PopulationYouthOptions Options { get; set; }
        public TimeSeries DataSeries { get; set; }

        public override void Update(object sender, EventArgs e)
        {
            Reset(TransactionSource.PopulationYouth);
            IUsageSubject usage = Subject as IUsageSubject;
            IHasTransactions trans = (Subject as IHasTransactions);
            if(Options.IsVillage == true || (Options.Karbari!= 1 && Options.Capacity < 1))
                return;
            if (new int[] { 7, 22, 33, 34 }.ToList<int>().Contains(Options.Karbari))
                return;

            InTime timeObj = usage.InTime();
            LinkedListNode<Dictionary<string, Block>> family = null;
            if (SerialNo != null)
            {
                var families = usage.Gaps.Select(new EntryHash(EntryType.Extend, Options.CityId, SerialNo.Value, (int)GapType.Family, 0, 0, 0), () =>
                {
                    Frame<Block> resolved = new Frame<Block>(EntryHash.Empty);
                    Block defaultFamily = new Block(usage.StartedAt, 0, 0, 1, "F", 0);
                    defaultFamily.SetDuration(usage.EndedAt);
                    defaultFamily.SetUsage(usage);
                    resolved.Add(defaultFamily);
                    return resolved;
                }).Extract<LinkedList<Dictionary<string, Block>>>(new Link(timeObj.From, timeObj.To, usage, 1));
                family = families.First;
            }
            
            double price = 0;
            while (family != null)
            {
                IUsageSubject familyUsage = family.Value["F"].Usage;
                double monthly = familyUsage.MonthlyAverage / Options.TedadVahed.ToDouble();
                InTime partialObj = familyUsage.InTime();

                //Addtional Rule
                Frame<Block> CHash = DataSeries.Select(new EntryHash(EntryType.Water, Options.CityId, 0, 14, 0, 0, 1)).Transform(new InVolume(familyUsage.MonthlyAverage / family.Value["F"].Value));
                double cityRate = CHash.LastOrDefault().Value;
                Frame<Block> Series = new Frame<Block>(EntryHash.Empty);
                Frame<Block> PYHash = DataSeries.Select(new EntryHash(EntryType.Water, 0, 0, 85, 0, 0, 1));
                Frame<Block> SRHash2 = DataSeries.Select(new EntryHash(EntryType.Water, Options.CityId, 11, 15, 0, 0, 1)).Transform(new InVolume(familyUsage.MonthlyAverage));
                double Overlap = 0.0;
                LinkedList<Dictionary<string, Block>> rates = SRHash2.Extract<LinkedList<Dictionary<string, Block>>>(new Link(partialObj.From, partialObj.To, familyUsage));
                LinkedListNode<Dictionary<string, Block>> currentStep = rates.First;
                int maxLoop1 = 10;
                while (currentStep != null)
                {
                    double rangeDuration = currentStep.Value.First().Value.Duration.Days;
                    if(Options.Karbari ==1)
                        Overlap += currentStep.Value["15"].Value/30.0 *familyUsage.Duration.Days.ToString().ToDouble();
                    else
                        Overlap += Options.Capacity / 30.0 * familyUsage.Duration.Days.ToString().ToDouble();
                    currentStep = currentStep.Next;
                    maxLoop1--;
                    if (maxLoop1 == 0)
                        break;
                }
                Overlap *= Options.TedadVahed;
                Overlap *= family.Value["F"].Value;
                double amount = usage.Consumption - Overlap;
                if (amount <= 0)
                    return;

                LinkedList<Dictionary<string, Block>> rates2 = Series.Transform(new MergeFrame(PYHash))
                            .Extract<LinkedList<Dictionary<string, Block>>>(new Link(partialObj.From, partialObj.To, familyUsage));
                LinkedListNode<Dictionary<string, Block>> currentStep2 = rates2.First;
                int maxLoop = 10;
                while (currentStep2 != null)
                {
                    price += (currentStep2.Value.First().Value.Usage.Consumption * currentStep2.Value["85"].Value);
                    maxLoop--;
                    if (maxLoop == 0)
                        break;
                    currentStep2 = currentStep2.Next;
                }
                family = family.Next;
            }
            Effect(new Transaction(TransactionSource.PopulationYouth, TransactionType.Debit, price));
            int[] codes = new int[] { 55, 79, 262, 97, 238, 271 };
            if (codes.Contains(Options.CategoryUses))
            {
                Effect(new Transaction(TransactionSource.PopulationYouth, TransactionType.Discount, price));
            }
        }
        
    }
}