using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.Core;
using Rocket.States;
using Rocket.Options;

namespace Rocket.Units
{
    public class WaterPattern : Unit, IObserver
    {
        public WaterPattern(TimeSeries series, WaterPatternOptions options, int? serialNo = null)
            : base()
        {
            Options = options;
            DataSeries = series;
            SerialNo = serialNo;
        }
        public int? SerialNo { get; set; }
        public WaterPatternOptions Options { get; set; }
        public TimeSeries DataSeries { get; set; }

        public override void Update(object sender, EventArgs e)
        {
            Reset(TransactionSource.WaterPattern);
            IUsageSubject usage = Subject as IUsageSubject;
            InTime timeObj0 = usage.InTime();
            var formula = DataSeries.Select(new EntryHash(EntryType.Water, 0, 17, 6, 0, 0, 0)).Extract<LinkedList<Dictionary<string, Block>>>(new Link(timeObj0.From, timeObj0.To, usage, 1));
            var action = formula.First;
            while (action != null)
            {
                var version = action.Value["6"].Value.ToString().ToInt();
                switch (version)
                {
                    case 0:
                    case 1:
                        usage = action.Value.First().Value.Usage;
                        timeObj0 = usage.InTime();
                        LinkedListNode<Dictionary<string, Block>> temporalHousing = null;
                        if (SerialNo != null)
                        {
                            var temporalHousings = usage.Gaps.Select(new EntryHash(EntryType.Extend, Options.CityId, SerialNo.Value, (int)GapType.TemporalHousing, 0, 0, 0), () =>
                            {
                                Frame<Block> resolved = new Frame<Block>(EntryHash.Empty);
                                Block defaultUnderConstruction = new Block(usage.StartedAt, 0, 0, 0, "G4", 0);
                                defaultUnderConstruction.SetDuration(usage.EndedAt);
                                defaultUnderConstruction.SetUsage(usage);
                                resolved.Add(defaultUnderConstruction);
                                return resolved;
                            }).Extract<LinkedList<Dictionary<string, Block>>>(new Link(timeObj0.From, timeObj0.To, usage, 0));
                            temporalHousing = temporalHousings.First;
                        }
                        if (temporalHousing.Value["G4"].Value > 0)
                            return;
                        /*InTime illegalTimeObj = usage.InTime();
                        var illegals = usage.Gaps.Select(new EntryHash(EntryType.Extend, Options.CityId, SerialNo.Value, (int)GapType.Illegal, 0, 0, 0), () =>
                        {
                            return new Frame<Block>(EntryHash.Empty);
                        }).Extract<LinkedList<Dictionary<string, Block>>>(new Link(illegalTimeObj.From, illegalTimeObj.To, usage, 1));
                        if (illegals.Count > 0)
                            return;*/
                        if (Options.Karbari == 1 && Options.IsVillage && Options.CategoryUses == 154)
                            return;
                        double price = 0;
                        bool type = (Options.Tariff == 11);
                        if (!type || Options.ReadCode == 5)
                            return;
                        DateTime start = usage.StartedAt;
                        DateTime finish = usage.EndedAt;
                        DateTime breakPoint = DateTime.Parse("2019-5-21");
                        if (start < breakPoint && finish > breakPoint)
                            usage = usage.SetTime(breakPoint, usage.EndedAt);
                        IUsageSubject PatternUsage = usage;

                        if (new double[] { 4, 6 }.Contains(Options.ReadCode))
                            PatternUsage = usage.Clone(null, null, null, usage.Consumption / Options.TedadVahed.ToDouble(), null, null);
                        else
                            PatternUsage = usage.Clone(null, null, null, usage.Consumption / Options.TedadVahed.ToDouble(), null, null);
                        InTime timeObj = PatternUsage.InTime();
                        LinkedListNode<Dictionary<string, Block>> underConstruction = null;
                        if (SerialNo != null)
                        {
                            var underConstructions = PatternUsage.Gaps.Select(new EntryHash(EntryType.Extend, Options.CityId, SerialNo.Value, (int)GapType.UnderConstruction, 0, 0, 0), () =>
                            {
                                Frame<Block> resolved = new Frame<Block>(EntryHash.Empty);
                                Block defaultUnderConstruction = new Block(PatternUsage.StartedAt, 0, 0, 0, "G1", 0);
                                defaultUnderConstruction.SetDuration(PatternUsage.EndedAt);
                                defaultUnderConstruction.SetUsage(PatternUsage);
                                resolved.Add(defaultUnderConstruction);
                                return resolved;
                            }).Extract<LinkedList<Dictionary<string, Block>>>(new Link(timeObj.From, timeObj.To, PatternUsage, 1));
                            underConstruction = underConstructions.First;
                        }
                        double patternPrice = 0.0;
                        double normalPrice = 0.0;
                        while (underConstruction != null)
                        {
                            if (underConstruction.Value.First().Value.Value > 0)
                            {
                                underConstruction = underConstruction.Next;
                                continue;
                            }
                            IUsageSubject underConstructionUsage = underConstruction.Value.First().Value.Usage;
                            InTime familyTimeObj = underConstructionUsage.InTime();
                            LinkedListNode<Dictionary<string, Block>> family = null;
                            if (SerialNo != null)
                            {
                                var families = underConstructionUsage.Gaps.Select(new EntryHash(EntryType.Extend, Options.CityId, SerialNo.Value, (int)GapType.Family, 0, 0, 0), () =>
                                {
                                    Frame<Block> resolved = new Frame<Block>(EntryHash.Empty);
                                    Block defaultFamily = new Block(underConstructionUsage.StartedAt, 0, 0, 1, "F", 0);
                                    defaultFamily.SetDuration(underConstructionUsage.EndedAt);
                                    defaultFamily.SetUsage(underConstructionUsage);
                                    resolved.Add(defaultFamily);
                                    return resolved;
                                }).Extract<LinkedList<Dictionary<string, Block>>>(new Link(familyTimeObj.From, familyTimeObj.To, underConstructionUsage, 1));
                                family = families.First;
                            }

                            while (family != null)
                            {
                                IUsageSubject familyUsage = family.Value["F"].Usage;
                                Frame<Block> Series = new Frame<Block>(EntryHash.Empty);
                                //Range Cost Rate Fetching
                                Frame<Block> RHash = DataSeries.Select(new EntryHash(EntryType.Water, Options.CityId, Options.Tariff, 15, 0, 0, 1)).Transform(new InVolume(familyUsage.MonthlyAverage / family.Value["F"].Value));
                                //Entry RRate = D[RHash].Slice(cloned);
                                //City Cost Fetching
                                Frame<Block> CHash = DataSeries.Select(new EntryHash(EntryType.Water, Options.CityId, 0, 14, 0, 0, 1)).Transform(new InVolume(familyUsage.MonthlyAverage / family.Value["F"].Value));
                                //Entry CRate = D[CHash].Slice(cloned);
                                Frame<Block> WPHash = DataSeries.Select(new EntryHash(EntryType.Water, Options.CityId, 110, 17, 0, 0, 1)).Transform(new InVolume(familyUsage.MonthlyAverage / family.Value["F"].Value));
                                //Entry WPRate = D[WPHash].Slice(cloned);
                                Frame<Block> WCHash = DataSeries.Select(new EntryHash(EntryType.Water, Options.CityId, Options.Tariff, 17, 0, 0, 1)).Transform(new InVolume(familyUsage.MonthlyAverage / family.Value["F"].Value));
                                //Entry WCRate = D[WCHash].Slice(cloned);
                                InTime partialObj = familyUsage.InTime();
                                LinkedList<Dictionary<string, Block>> rates =
                                    Series.Transform(new MergeFrame(RHash)).Transform(new MergeFrame(CHash)).Transform(new MergeFrame(WPHash)).Transform(new MergeFrame(WCHash))
                                        .Extract<LinkedList<Dictionary<string, Block>>>(new Link(partialObj.From, partialObj.To, familyUsage));// (D.GetFamily(cloned) + RRate + CRate + WCRate + WPRate).GetBlocks(cloned);
                                LinkedListNode<Dictionary<string, Block>> currentStep = rates.First;

                                int maxLoop = 10;
                                while (currentStep != null)
                                {
                                    IUsageSubject currentUsage = currentStep.Value.First().Value.Usage;
                                    if ((familyUsage.MonthlyAverage / family.Value["F"].Value) <= currentStep.Value["15"].Value)
                                    {
                                        currentStep = currentStep.Next;
                                        continue;
                                    }
                                    InTime partailTime2 = currentUsage.InTime();
                                    LinkedListNode<Dictionary<string, Block>> illegal = null;
                                    if (SerialNo != null)
                                    {
                                        var illegals = usage.Gaps.Select(new EntryHash(EntryType.Extend, Options.CityId, SerialNo.Value, (int)GapType.Illegal, 0, 0, 0), () =>
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
                                        IUsageSubject illegalUsage = illegal.Value.First().Value.Usage;
                                        double rangeDuration = illegal.Value.First().Value.Duration.Days.ToString().ToDouble();
                                        if (illegal.Value.First().Value.Value > 0)
                                        {
                                            illegal = illegal.Next;
                                            continue;
                                        }
                                        Overlap += illegalUsage.Consumption;

                                        patternPrice += (currentStep.Value["WP17"].Value * (illegalUsage.MonthlyAverage / family.Value["F"].Value) - currentStep.Value["WP17"].Extra) * (rangeDuration / 30) * currentStep.Value["14"].Value * family.Value["F"].Value;
                                        normalPrice += (currentStep.Value["17"].Value * (illegalUsage.MonthlyAverage / family.Value["F"].Value) - currentStep.Value["17"].Extra) * (rangeDuration / 30) * currentStep.Value["14"].Value * family.Value["F"].Value;

                                        illegal = illegal.Next;
                                    }

                                    currentStep = currentStep.Next;
                                    maxLoop--;
                                    if (maxLoop == 0)
                                        break;
                                }
                                family = family.Next;
                            }
                            underConstruction = underConstruction.Next;
                        }

                        if (normalPrice <= 0 || patternPrice <= 0 || normalPrice > patternPrice)
                            return;

                        price += (patternPrice - normalPrice) * Options.TedadVahed;

                        if (Options.IsVillage && Options.Tariff != 24)
                            Effect(new Transaction(TransactionSource.WaterPattern, TransactionType.Debit, price / 2, ""));
                        else
                            Effect(new Transaction(TransactionSource.WaterPattern, TransactionType.Debit, price));
                        break;
                    case 2:
                        return;
                }


                action = action.Next;
            }

        }
        public double Overlap { get; private set; }
    }
}