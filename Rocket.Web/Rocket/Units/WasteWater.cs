using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.Core;
using Rocket.States;
using Rocket.Options;

namespace Rocket.Units
{
    public class WasteWater: Unit, IObserver
    {
        public WasteWater(TimeSeries series, WasteWaterOptions options, int? serialNo = null)
            : base()
        {
            Options = options;
            DataSeries = series;
            SerialNo = serialNo;
        }
        public int? SerialNo { get; set; }
        public WasteWaterOptions Options { get; set; }
        public TimeSeries DataSeries { get; set; }

        public override void Update(object sender, EventArgs e)
        {
            Reset(TransactionSource.WasteWater);
            IUsageSubject usage = Subject as IUsageSubject;
            if (Options.Karbari == 1 && Options.IsVillage && Options.CategoryUses == 154)
                return;
            double price = 0;
            if (Options.KindBranch == 2 || Options.CategoryUses == 77 || new double[] { 22, 23 }.Contains(Options.Tariff))
                return;
            double rate_per_day = 0;
            IHasTransactions trans = Subject as IHasTransactions;
            double transit = 0;
            double balance = 0;
            double discount = trans.Transactions.Where(item => item.Type == TransactionType.Discount && item.Source == TransactionSource.WaterCost && item.Description == "مخفف الگوی مدارس").Sum(item => item.Amount);
            if (Options.Karbari == 1)
            {
                transit = trans.Transit(TransactionSource.WaterCost) + trans.Transit(TransactionSource.WaterPattern);
                balance = (trans.Balance(TransactionSource.WaterCost) + trans.Balance(TransactionSource.WaterPattern) + trans.Balance(TransactionSource.Seasonal));
                
            }
            else
            {
                transit = trans.Transit(TransactionSource.WaterCost) + trans.Transit(TransactionSource.WaterPattern);
                balance = (trans.Balance(TransactionSource.WaterCost) + trans.Balance(TransactionSource.WaterPattern)  + trans.Balance(TransactionSource.FreeWater) + trans.Balance(TransactionSource.Seasonal));
            }
            rate_per_day = (balance + discount) - transit; 
            if (rate_per_day == 0)
                return;
            double effectedDuration = 0;
            //rate_per_day /= usage.Duration.Days.ToDouble();
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
                }).Extract<LinkedList<Dictionary<string, Block>>>(new Link(timeUnderObj.From, timeUnderObj.To, usage, 0));
                underConstruction = underConstructions.First;
            }
            while (underConstruction != null)
            {
                if (underConstruction.Value.First().Value.Value == 1)
                {
                    underConstruction = underConstruction.Next;
                    continue;
                }
                IUsageSubject underConstructionUsage = underConstruction.Value.First().Value.Usage;
                InTime illegalTimeObj = underConstructionUsage.InTime();
                LinkedListNode<Dictionary<string, Block>> illegal = null;
                if (SerialNo != null)
                {
                    var illegals = underConstructionUsage.Gaps.Select(new EntryHash(EntryType.Extend, Options.CityId, SerialNo.Value, (int)GapType.Illegal, 0, 0, 0), () =>
                    {
                        Frame<Block> resolved = new Frame<Block>(EntryHash.Empty);
                        Block defaultIllegal = new Block(illegalTimeObj.From, 0, 9999999, 0, "I", 0);
                        defaultIllegal.SetDuration(illegalTimeObj.To);
                        defaultIllegal.SetUsage(underConstructionUsage);
                        resolved.Add(defaultIllegal);
                        return resolved;
                    }).Extract<LinkedList<Dictionary<string, Block>>>(new Link(illegalTimeObj.From, illegalTimeObj.To, underConstructionUsage, 0));
                    illegal = illegals.First;
                }
                while (illegal != null) {
                    /*if (illegal.Value.First().Value.Value > 0)
                    {
                        illegal = illegal.Next;
                        continue;
                    }*/
                    effectedDuration += illegal.Value.First().Value.Usage.Duration.Days.ToDouble();
                    illegal = illegal.Next;
                }
                underConstruction = underConstruction.Next;
            }
            rate_per_day /= effectedDuration;
            timeUnderObj = usage.InTime();
            underConstruction = null;
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
                }).Extract<LinkedList<Dictionary<string, Block>>>(new Link(timeUnderObj.From, timeUnderObj.To, usage, 0));
                underConstruction = underConstructions.First;
            }
            while (underConstruction != null)
            {
                if (underConstruction.Value.First().Value.Value == 1)
                {
                    underConstruction = underConstruction.Next;
                    continue;
                }
                IUsageSubject underConstructionUsage = underConstruction.Value.First().Value.Usage;
                InTime timeObj = underConstructionUsage.InTime();
                Frame<Block> Series = new Frame<Block>(EntryHash.Empty);
                Frame<Block> WWHash = DataSeries.Select(new EntryHash(EntryType.Water, Options.CityId, Options.Karbari == 1 ? 11 : Options.Tariff, 13, 0, 0, 1)).Transform(new InVolume(underConstructionUsage.MonthlyAverage));
                //Entry WWRate = D[WWHash].Slice(Options);
                LinkedList<Dictionary<string, Block>> rates = Series.Transform(new MergeFrame(WWHash))
                        .Extract<LinkedList<Dictionary<string, Block>>>(new Link(timeObj.From, timeObj.To, underConstructionUsage));
                LinkedListNode<Dictionary<string, Block>> currentStep = rates.First;
                int maxLoop = 10;
                while (currentStep != null)
                {
                    IUsageSubject currentUsage = currentStep.Value.First().Value.Usage;
                    InTime partailTime2 = currentUsage.InTime();
                    LinkedListNode<Dictionary<string, Block>> illegal = null;
                    if (SerialNo != null)
                    {
                        var illegals = currentUsage.Gaps.Select(new EntryHash(EntryType.Extend, Options.CityId, SerialNo.Value, (int)GapType.Illegal, 0, 0, 0), () =>
                        {
                            Frame<Block> resolved = new Frame<Block>(EntryHash.Empty);
                            Block defaultIllegal = new Block(partailTime2.From, 0, 9999999, 0, "I", 0);
                            defaultIllegal.SetDuration(partailTime2.To);
                            defaultIllegal.SetUsage(currentUsage);
                            resolved.Add(defaultIllegal);
                            return resolved;
                        }).Extract<LinkedList<Dictionary<string, Block>>>(new Link(partailTime2.From, partailTime2.To, currentUsage, 0));
                        illegal = illegals.First;
                    }
                    while (illegal != null)
                    {
                        /*if (illegal.Value.First().Value.Value > 0)
                        {
                            illegal = illegal.Next;
                            continue;
                        }*/
                        IUsageSubject illegalUsage = illegal.Value.First().Value.Usage;
                        double rangeDuration = illegal.Value.First().Value.Duration.Days.ToString().ToDouble();
                        double factor = currentStep.Value["13"].Value;
                        if (illegal.Value.First().Value.Value > 0)
                            factor = 1;
                        if (Options.CityId==16 && SerialNo != null && SerialNo == 8726)
                        {
                            factor = 0.7;
                        }
                        price += rate_per_day * rangeDuration * factor;
                        illegal = illegal.Next;
                    }
                    
                    
                    currentStep = currentStep.Next;
                    maxLoop--;
                    if (maxLoop == 0)
                        break;
                }
                underConstruction = underConstruction.Next;
            } 
            
            Effect(new Transaction(TransactionSource.WasteWater, TransactionType.Debit, price));
            int[] codes = { 55, 79, 262, 97, 238, 271 };
            if (codes.Contains(Options.CategoryUses))
            {
                Effect(new Transaction(TransactionSource.WasteWater, TransactionType.Discount, price));
            }
            
        }
        
    }
}