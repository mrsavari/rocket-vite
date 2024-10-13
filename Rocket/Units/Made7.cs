using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.Core;
using Rocket.States;
using Rocket.Options;

namespace Rocket.Units
{
    public class Made7: Unit, IObserver
    {
        public Made7(TimeSeries series, Made7Options options, int? serialNo = null)
            : base()
        {
            Options = options;
            DataSeries = series;
            SerialNo = serialNo;
        }
        public int? SerialNo { get; set; }
        public Made7Options Options { get; set; }
        public TimeSeries DataSeries { get; set; }

        public override void Update(object sender, EventArgs e)
        {
            Reset(TransactionSource.Made7);
            IUsageSubject usage = Subject as IUsageSubject;
            if (Options.Karbari == 1 && Options.IsVillage && Options.CategoryUses == 154)
                return;
            if (Options.Karbari == 7)
            {
                if (Options.CityId == 16 || Options.CityId == 21 || Options.CityId == 69)
                    return;
            }
            int[] codes = { 55, 79, 262, 238, 97, 271, 1, 59, 58, 57, 56, 63, 62, 61, 60, 70, 152, 80, 35, 82, 83, 84, 156 };
            int[] tariffs = { 23, 24,7, 9 };
            if (Options.IsVillage || codes.Contains(Options.CategoryUses) || tariffs.Contains(Options.Tariff))
                return;
            double price = 0;
            if (Options.CityId == 21 || Options.CityId == 69)
            {
                if (Options.Karbari == 2)
                {
                    double freeAmount = 0;
                    try
                    {
                        freeAmount = (usage.State as OverUsageState).GetExtraUsage();
                    }
                    catch { }
                    usage = usage.Clone(null,null,null,freeAmount,null,null);
                }
            }
            double amount = usage.Consumption;
            if (DateTime.Parse("2019-03-12") < DateTime.Now && DateTime.Now < DateTime.Parse("2020-03-13") && Options.CityId == 60 && (Options.CategoryUses == 328 || Options.CategoryUses == 69))
            {
                if (Options.CategoryUses == 328)
                    price = amount * 4000;
                if (Options.CategoryUses == 69)
                    price = amount * 4500;
                Effect(new Transaction(TransactionSource.Made7,TransactionType.Debit,price));
                return;
            }
            InTime timeUnderObj = usage.InTime();
            LinkedListNode<Dictionary<string, Block>> underConstruction = null;
            if (SerialNo != null)
            {
                var underConstructions = usage.Gaps.Select(new EntryHash(EntryType.Extend, Options.CityId, SerialNo.Value, (int)GapType.UnderConstruction, 0, 0, 0), () =>
                {
                    Frame<Block> resolved = new Frame<Block>(EntryHash.Empty);
                    Block defaultUnderConstruction = new Block(usage.StartedAt, 0, 0, 0, "G1", 0);
                    defaultUnderConstruction.SetDuration(usage.EndedAt);
                    defaultUnderConstruction.SetUsage(usage);
                    resolved.Add(defaultUnderConstruction);
                    return resolved;
                }).Extract<LinkedList<Dictionary<string, Block>>>(new Link(timeUnderObj.From, timeUnderObj.To, usage, 1));
                underConstruction = underConstructions.First;
            }
            while (underConstruction != null)
            {
                IUsageSubject underConstructionUsage = underConstruction.Value.First().Value.Usage;
                InTime illegalTimeObj = underConstructionUsage.InTime();
                LinkedListNode<Dictionary<string, Block>> illegal = null;
                if (SerialNo != null)
                {
                    var illegals = usage.Gaps.Select(new EntryHash(EntryType.Extend, Options.CityId, SerialNo.Value, (int)GapType.Illegal, 0, 0, 0), () =>
                    {
                        Frame<Block> resolved = new Frame<Block>(EntryHash.Empty);
                        Block defaultIllegal = new Block(illegalTimeObj.From, 0, 9999999, 0, "I", 0);
                        defaultIllegal.SetDuration(illegalTimeObj.To);
                        defaultIllegal.SetUsage(underConstructionUsage);
                        resolved.Add(defaultIllegal);
                        return resolved;
                    }).Extract<LinkedList<Dictionary<string, Block>>>(new Link(illegalTimeObj.From, illegalTimeObj.To, underConstructionUsage, 1));
                    illegal = illegals.First;
                }
                while (illegal != null)
                {
                    IUsageSubject illegalUsage = illegal.Value.First().Value.Usage;
                    InTime timeObj = illegalUsage.InTime();
                    Frame<Block> Series = new Frame<Block>(EntryHash.Empty);
                    Frame<Block> M7Hash = DataSeries.Select(new EntryHash(EntryType.Water, Options.CityId, Options.Karbari, 24, Options.CategoryUses == 69 ? 69 : 0, 0, 1)).Transform(new InVolume(illegalUsage.MonthlyAverage));
                    //Entry M7Rate = D[M7Hash].Slice(Options);
                    LinkedList<Dictionary<string, Block>> rates = Series.Transform(new MergeFrame(M7Hash))
                            .Transform(new MergeFrame(DataSeries.GetFamily(illegalUsage, Options.TedadKhanvar)))
                            .Extract<LinkedList<Dictionary<string, Block>>>(new Link(timeObj.From, timeObj.To, illegalUsage));
                    LinkedListNode<Dictionary<string, Block>> currentStep = rates.First;
                    int maxLoop = 10;
                    if (currentStep.Value.ContainsKey("24") == false)
                    {
                        illegal = illegal.Next;
                        continue;
                    }
                        
                    while (currentStep != null)
                    {
                        IUsageSubject currentlUsage = currentStep.Value.First().Value.Usage;
                        if (Options.Karbari != 1 && new int[] { 58, 8, 11, 57, 25, 22, 28, 16, 21, 69 }.Contains(Options.CityId) && (underConstruction.Value.First().Value.Value > 0 || illegal.Value.First().Value.Value > 0))
                        {
                            currentStep = currentStep.Next;
                            continue;
                        }

                        double rangeDuration = double.Parse(currentStep.Value.First().Value.Duration.Days.ToString());
                        if (Options.ReadCode == 5)
                        {
                            amount = currentlUsage.MonthlyAverage / currentStep.Value["F"].Value;
                            currentlUsage = currentlUsage.Clone(null, null, null, amount, null, null);
                        }
                        else
                        {
                            amount = currentlUsage.Consumption / currentStep.Value["F"].Value;
                            if (currentStep.Value["F"].Value > 1)
                                amount = currentlUsage.MonthlyAverage / currentStep.Value["F"].Value;
                        }
                        //if (Options.Karbari == 1 && Options.TedadVahed > 1 && new double[] { 4, 6 }.Contains(Options.ReadCode))
                        //    amount *= Options.TedadVahed;
                        price += ((rangeDuration / 30.0) * currentStep.Value["24"].Extra) * currentStep.Value["F"].Value;
                        price += ((amount * rangeDuration / currentlUsage.Duration.Days.ToDouble()) * currentStep.Value["24"].Value) * currentStep.Value["F"].Value;
                        currentStep = currentStep.Next;
                        maxLoop--;
                        if (maxLoop == 0)
                            break;
                    }
                    illegal = illegal.Next;
                }
                
                
                underConstruction = underConstruction.Next;
            }
            
            Effect(new Transaction(TransactionSource.Made7,TransactionType.Debit,price));
        }
        
    }
}