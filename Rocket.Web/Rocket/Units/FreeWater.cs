using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.Core;
using Rocket.States;
using Rocket.Options;

namespace Rocket.Units
{
    public class FreeWater: Unit, IObserver
    {
        public FreeWater(TimeSeries series, FreeWaterOptions options, int? serialNo = null)
            : base()
        {
            Options = options;
            DataSeries = series;
            SerialNo = serialNo;
        }

        public int? SerialNo { get; set; }
        public FreeWaterOptions Options { get; set; }
        public TimeSeries DataSeries { get; set; }

        public override void Update(object sender, EventArgs e)
        {
            Reset(TransactionSource.FreeWater);
            double temporalFree = 0.0;

            IUsageSubject usage = Subject as IUsageSubject;
            InTime timeObj = usage.InTime();
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
                }).Extract<LinkedList<Dictionary<string, Block>>>(new Link(timeObj.From, timeObj.To, usage, 0));
                temporalHousing = temporalHousings.First;
            }

            if ((temporalHousing != null && temporalHousing.Value["G4"].Value > 0))
                return;

            while (temporalHousing != null && temporalHousing.Value["G4"].Value > 0)
            {
                double value = Options.IsVillage ? 5 : temporalHousing.Value["G4"].Value;
                double consumption = temporalHousing.Value["G4"].Usage.Consumption;
                double allowed = value / 30.0 * temporalHousing.Value["G4"].Usage.Duration.Days.ToDouble();
                if(consumption > allowed)
                    temporalFree += (consumption - allowed);
                temporalHousing = temporalHousing.Next;
            }

            if (Options.Karbari == 1 && temporalFree == 0)
                return;

            if (Options.Karbari == 1 && temporalFree == 0 && Options.IsVillage && Options.CategoryUses == 154)
                return;

            InTime someTime = usage.InTime();
            /*if (SerialNo != null)
            {
                var illegals = usage.Gaps.Select(new EntryHash(EntryType.Extend, Options.CityId, SerialNo.Value, (int)GapType.Illegal, 0, 0, 0), () =>
                {
                    return new Frame<Block>(EntryHash.Empty);
                }).Extract<LinkedList<Dictionary<string, Block>>>(new Link(someTime.From, someTime.To, usage, 1));
                if (illegals.Count > 0)
                {
                    return;
                }
            }*/
            if ((usage.State as UsageState).GetExtraUsage() == 0)
            {
                return;
            }
            if (new double[] { 24, 5 }.Contains(Options.Tariff) == false && new double[] { 4, 21, 29, 33, 34, 7, 22,  31 }.Contains(Options.Karbari))
                return;
            if (Options.Karbari == 7)
                return;
            if (Options.Tariff == 24)
                return;

            if (new double[] { 409 }.Contains(Options.CategoryUses))
                return;

            double freeAmount = 0;
            try
            {
                if (temporalFree > 0)
                    freeAmount = temporalFree;
                else
                    freeAmount = (usage.State as OverUsageState).GetExtraUsage();
            }
            catch { }
            if (freeAmount < 0)
                return;

            UsageSubject cloned = usage.Clone(null,null,null,freeAmount,null,null);

            timeObj = cloned.InTime();
            Frame<Block> Series = new Frame<Block>(EntryHash.Empty);
            Frame<Block> FWHash = DataSeries.Select(new EntryHash(EntryType.Water, Options.CityId, 5, 17, 0, 0, 1)).Transform(new InVolume(usage.MonthlyAverage));
            Frame<Block> CHash = DataSeries.Select(new EntryHash(EntryType.Water, Options.CityId, 0, 14, 0, 0, 1)).Transform(new InVolume(usage.MonthlyAverage));
            LinkedList<Dictionary<string, Block>> rates = Series.Transform(new MergeFrame(FWHash))
                    .Transform(new MergeFrame(CHash))
                    .Extract<LinkedList<Dictionary<string, Block>>>(new Link(timeObj.From, timeObj.To, usage));
            LinkedListNode<Dictionary<string, Block>> currentStep = rates.First;
            double price = 0;
            int maxLoop = 10;
            while (currentStep != null)
            {
                IUsageSubject currentUsage = currentStep.Value.First().Value.Usage;
                double rangeDuration = currentStep.Value.First().Value.Duration.Days.ToString().ToDouble();
                ApplyCityRate applyCityRate = new ApplyCityRate(DataSeries);
                double priceItem = applyCityRate.Apply(currentUsage, Options.CityId, Options.Tariff, Options.IsVillage, 1.0, (r, cityDuration) =>
                {
                    return freeAmount * (cityDuration / usage.Duration.Days.ToDouble()) * currentStep.Value["17"].Value * ((r < 1) ? 1 : r);
                    //return illegalUsage.Consumption * cost * r * (cityDuration / currentStep.Duration.Days.ToDouble());
                });
                price += priceItem;//freeAmount * (rangeDuration / usage.Duration.Days.ToDouble()) * currentStep.Value["17"].Value * ((currentStep.Value["14"].Value < 1) ? 1 : currentStep.Value["14"].Value);
                currentStep = currentStep.Next;
                maxLoop--;
                if (maxLoop == 0)
                    break;
            }
            Effect(new Transaction(TransactionSource.FreeWater,TransactionType.Debit,price));
            int[] categories = { 55, 79, 262, 97, 238, 271 };
            if (categories.Contains(Options.CategoryUses))
            {
                Effect(new Transaction(TransactionSource.FreeWater,TransactionType.Discount,price));
            }
        }
        
    }
}