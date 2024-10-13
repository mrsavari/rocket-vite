using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.Core;
using Rocket.Options;

namespace Rocket.Units
{
    public class SixthRemark : Unit, IObserver
    {
        public SixthRemark(TimeSeries series, SixthRemarkOptions options, int? serialNo = null)
            : base()
        {
            Options = options;
            DataSeries = series;
            SerialNo = serialNo;
        }

        public int? SerialNo { get; set; }
        public SixthRemarkOptions Options { get; set; }
        public TimeSeries DataSeries { get; set; }

        public override void Update(object sender, EventArgs e)
        {
            Reset(TransactionSource.SixthRemark);
            IUsageSubject usage1 = Subject as IUsageSubject;
            InTime partailTime1 = usage1.InTime();
            var formula = DataSeries.Select(new EntryHash(EntryType.Water, 0, 76, 6, 0, 0, 0)).Extract<LinkedList<Dictionary<string, Block>>>(new Link(partailTime1.From, partailTime1.To, usage1, 1));
            var action = formula.First;
            while (action != null)
            {
                IUsageSubject usage = action.Value.First().Value.Usage;
                var version = action.Value["6"].Value.ToString().ToInt();
                switch (version)
                {
                    case 0:
                    case 1:
                        IHasTransactions trans = (Subject as IHasTransactions);
                        //if (Options.IsVillage == true)
                        //    return;
                        if (Options.Karbari == 1 && Options.CategoryUses == 154)
                            return;
                        if (new int[] { 31, 21, 26, 30 }.ToList<int>().Contains(Options.Karbari))
                            return;
                        int[] tariffs = { 23, 7, 9 };
                        int[] codes = new int[] { 66, 76 };
                        if (codes.ToList<int>().Contains(Options.CityId) && tariffs.ToList<int>().Contains(Options.Tariff))
                            return;
                        int costTypeId = 76;
                        if (Options.Karbari != 1)
                            costTypeId = 77;
                        if (Options.Karbari == 1 && Options.IsVillage && Options.CategoryUses == 154)
                            return;

                        double waterPrice = trans.Balance(TransactionSource.WaterCost);// +trans.Balance(TransactionSource.Seasonal); // trans.Balance(TransactionSource.FreeWater) + trans.Balance(TransactionSource.WaterPattern)
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
                            //Entry SRRate = D[SRHash].Slice(cloned,false);

                            double amount = familyUsage.Consumption;
                            double amount_per_day = amount / double.Parse(familyUsage.Duration.Days.ToString());
                            //Addtional Rule
                            #region قانون بودجه سال ۹۹
                            DateTime Rule1 = DateTime.Parse("2020-03-19 00:00:00.000"); //1399-01-01
                            DateTime BillStart = familyUsage.StartedAt;
                            DateTime BillEnd = familyUsage.EndedAt;
                            Frame<Block> CHash = DataSeries.Select(new EntryHash(EntryType.Water, Options.CityId, 0, 14, 0, 0, 1)).Transform(new InVolume(familyUsage.MonthlyAverage / family.Value["F"].Value));
                            double cityRate = CHash.LastOrDefault().Value;
                            Frame<Block> Series = new Frame<Block>(EntryHash.Empty);
                            Frame<Block> SRHash = DataSeries.Select(new EntryHash(EntryType.Water, Options.CityId, Options.Tariff, costTypeId, 0, 0, 1));
                            Frame<Block> SRHash2 = DataSeries.Select(new EntryHash(EntryType.Water, Options.CityId, 11, 15, 0, 0, 1)).Transform(new InVolume(familyUsage.MonthlyAverage));
                            double Overlap = 0.0;
                            LinkedList<Dictionary<string, Block>> rates = SRHash2.Extract<LinkedList<Dictionary<string, Block>>>(new Link(partialObj.From, partialObj.To, familyUsage));
                            LinkedListNode<Dictionary<string, Block>> currentStep = rates.First;
                            int maxLoop1 = 10;
                            while (currentStep != null)
                            {
                                double rangeDuration = currentStep.Value.First().Value.Duration.Days;
                                Overlap += currentStep.Value["15"].Value / 30.0 * familyUsage.Duration.Days.ToString().ToDouble();// (rangeDuration / familyUsage.Duration.Days) * ;
                                currentStep = currentStep.Next;
                                maxLoop1--;
                                if (maxLoop1 == 0)
                                    break;
                            }
                            Overlap *= Options.TedadVahed;
                            Overlap *= family.Value["F"].Value;
                            if (familyUsage.EndedAt > DateTime.Parse("2022-03-21 00:00:00.000"))
                            {
                                if (new int[] { 5, 24, 23, }.Contains(Options.Tariff) || 80 == Options.CategoryUses || 9 == Options.Karbari)
                                    return;
                                if (Options.Tariff == 11)
                                {
                                    double range = familyUsage.Consumption - Overlap;
                                    double over_range = 0;
                                    if (range <= 0)
                                    {
                                        return;
                                    }
                                    if (range > Overlap)
                                    {
                                        over_range = range - (Overlap);
                                        range = Overlap;
                                    }
                                    double unit = waterPrice / usage.Consumption;
                                    price += ((range * 0.15) + (over_range * 0.35)) * unit;
                                }
                                else
                                {
                                    if (Options.Capacity <= 0.0)
                                        return;
                                    Frame<Block> CostHash = DataSeries.Select(new EntryHash(EntryType.Water, Options.CityId, Options.Tariff, 17, 0, 0, 1));
                                    double capacity = (Options.Capacity / 30 * usage.Duration.Days.ToDouble());
                                    double range = usage.Consumption - capacity;
                                    double over_range = 0;
                                    if (range <= 0)
                                    {
                                        return;
                                    }
                                    if (range > capacity)
                                    {
                                        over_range = range - (capacity);
                                        range = capacity;
                                    }
                                    double unit = waterPrice / capacity; //CostHash.LastOrDefault().Value;
                                    cityRate = 1;//cityRate < 1 ? 1 : cityRate;
                                    price += ((range * 0.15) + (over_range * 0.35)) * unit * cityRate;
                                }
                            }
                            else if (familyUsage.EndedAt > Rule1)
                            {
                                //Entry SRRate2 = D[SRHash2].Slice(cloned);

                                double max = (familyUsage.Karbari == 1) ? Overlap : familyUsage.Capacity;
                                if (new double[] { 4, 7, 22, 28 }.Contains(familyUsage.Karbari))
                                    max = 0;
                                if (BillStart > Rule1)
                                {
                                    if (max != 0 && (monthly * Options.TedadVahed) <= max)
                                    {
                                        return;
                                    }
                                    amount = ((monthly * Options.TedadVahed) - max) / 30 * familyUsage.Duration.Days.ToString().ToDouble();
                                }
                                else
                                {
                                    TimeSpan D1 = Rule1 - BillStart;
                                    TimeSpan D2 = familyUsage.EndedAt - Rule1;

                                    amount = ((monthly / family.Value["F"].Value) / 30.0).ToString("0.##########").ToDouble() * D1.Days.ToString().ToDouble();
                                    if ((monthly / family.Value["F"].Value) > max)
                                    {
                                        amount += ((((monthly / family.Value["F"].Value) - max) / 30.0).ToString("0.##########").ToDouble()) * D2.Days.ToString().ToDouble();
                                    }
                                }
                                amount_per_day = amount / double.Parse(familyUsage.Duration.Days.ToString());
                            }
                            #endregion
                            else
                            {

                                //if (tedad_vahed > 1 && new double[] { 4, 6 }.Contains(cloned.GetNumber("code_mane")))
                                //    amount_per_day *= tedad_vahed;

                                LinkedList<Dictionary<string, Block>> rates2 = Series.Transform(new MergeFrame(SRHash))
                                        .Extract<LinkedList<Dictionary<string, Block>>>(new Link(partialObj.From, partialObj.To, familyUsage));
                                LinkedListNode<Dictionary<string, Block>> currentStep2 = rates2.First;
                                int maxLoop = 10;
                                while (currentStep2 != null)
                                {
                                    IUsageSubject illegalUsage = currentStep2.Value.First().Value.Usage;
                                    InTime illegalTime = illegalUsage.InTime();
                                    LinkedListNode<Dictionary<string, Block>> illegal = null;
                                    var illegals = usage.Gaps.Select(new EntryHash(EntryType.Extend, Options.CityId, SerialNo.Value, (int)GapType.Illegal, 0, 0, 0), () =>
                                    {
                                        Frame<Block> resolved = new Frame<Block>(EntryHash.Empty);
                                        Block defaultIllegal = new Block(illegalTime.From, 0, 9999999, 0, "I", 0);
                                        defaultIllegal.SetDuration(illegalTime.To);
                                        defaultIllegal.SetUsage(illegalUsage);
                                        resolved.Add(defaultIllegal);
                                        return resolved;
                                    }).Extract<LinkedList<Dictionary<string, Block>>>(new Link(illegalTime.From, illegalTime.To, illegalUsage, 1));
                                    illegal = illegals.First;
                                    while (illegal != null)
                                    {
                                        double rangeDuration = double.Parse(illegal.Value.First().Value.Duration.Days.ToString()) / familyUsage.Duration.Days.ToDouble();

                                        price += amount_per_day * (familyUsage.Duration.Days.ToDouble() * rangeDuration) * currentStep2.Value[costTypeId.ToString()].Value * family.Value["F"].Value;
                                        illegal = illegal.Next;
                                        maxLoop--;
                                        if (maxLoop == 0)
                                            break;
                                    }
                                    currentStep2 = currentStep2.Next;

                                }
                            }
                            family = family.Next;
                        }
                        Effect(new Transaction(TransactionSource.SixthRemark, TransactionType.Debit, price));
                        codes = new int[] { 55, 79, 262, 97, 238, 271 };
                        if (codes.Contains(Options.CategoryUses))
                        {
                            Effect(new Transaction(TransactionSource.SixthRemark, TransactionType.Discount, price));
                        }
                        break;
                    }
                    action = action.Next;
                }
            }
    }
}