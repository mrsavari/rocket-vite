using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.Core;
using Rocket.States;
using Rocket.Options;

namespace Rocket.Units
{
    public class WaterCost : Unit
    {
        public WaterCost(TimeSeries series, WaterCostOptions options, int? serialNo = null)
            : base()
        {
            Options = options;
            DataSeries = series;
            SerialNo = serialNo;
        }

        public int? SerialNo { get; set; }
        public WaterCostOptions Options { get; set; }
        public TimeSeries DataSeries { get; set; }
        UsageState State { get; set; }

        public override void Update(object sender, EventArgs e)
        {
            Reset(TransactionSource.WaterCost);
            IUsageSubject usage = Subject as IUsageSubject;
            //if (Options.Karbari == 1 && Options.IsVillage && Options.CategoryUses == 154)
            //    return;
            int[] codes = { 55, 79, 262, 97, 238, 271 };
            if (Options.TedadVahed > 1)
                usage = usage.Clone(null, null, null, usage.Consumption / Options.TedadVahed.ToDouble(), null, null);
            double freeAmount = (usage.State as UsageState).GetExtraUsage();

            InTime someTime = usage.InTime();
            /*if (SerialNo != null)
            {
                var illegals = usage.Gaps.Select(new EntryHash(EntryType.Extend, Options.CityId, SerialNo.Value, (int)GapType.Illegal, 0, 0, 0), () =>
                {
                    return new Frame<Block>(EntryHash.Empty);
                }).Extract<LinkedList<Dictionary<string, Block>>>(new Link(someTime.From, someTime.To, usage, 1));
                if (illegals.Count > 0)
                {
                    Options.Tariff = 5;
                }
            }*/

            bool disable_capacity = false;
            bool disable_capacity_Karbari = new double[] { 4, 21, 29, 33, 34, 7, 22, 31 }.Contains(Options.Karbari);
            bool disable_capacity_TariffUsesId = new double[] { 24, 5 }.Contains(Options.Tariff);
            bool disable_capacity_CategoryUsesNo = new double[] { 409 }.Contains(Options.CategoryUses);
            disable_capacity = disable_capacity_Karbari || disable_capacity_TariffUsesId || disable_capacity_CategoryUsesNo;
            if (disable_capacity)
                freeAmount = 0;


            if (codes.Contains(Options.CategoryUses))
            {
                if (Options.Karbari != 1 && freeAmount > 0)
                    usage = usage.Clone(null, null, null, Math.Abs(usage.Consumption - freeAmount), null, null);
            }
            else
            {
                if (freeAmount > 0 && Options.Karbari != 1 && disable_capacity == false && usage.Capacity > 0)
                    usage = usage.Clone(null, null, null, Math.Abs(usage.Consumption - freeAmount), null, null);
            }
            if (usage.Consumption == 0)
                return;
            if (Options.Karbari != 1 && disable_capacity == false && usage.Capacity == 0)
            {
                return;
            }
            InTime timeObj = usage.InTime();
            var RHash = DataSeries.Select(new EntryHash(EntryType.Water, Options.CityId, Options.Tariff, 15, 0, 0, 1)).Transform(new InVolume(usage.MonthlyAverage)).Extract<LinkedList<Dictionary<string, Block>>>(new Link(timeObj.From, timeObj.To, usage, 1));
            double patternLimit = (RHash.Count == 0) ? usage.Capacity : RHash.Last.Value["15"].Value;

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
            int tariff = Options.Tariff;
            int tariffToShow = tariff;
            double price = 0;
            while (temporalHousing != null)
            {
                int tariff1 = temporalHousing.Value["G4"].Value == 0 ? Options.Tariff : 87;
                tariffToShow = tariff1;
                IUsageSubject temporalUsage = temporalHousing.Value["G4"].Usage;
                if (temporalHousing.Value["G4"].Value > 0)
                {
                    /*double consumption = temporalHousing.Value["G4"].Usage.Consumption;
                    double value = Options.IsVillage ? 5 : temporalHousing.Value["G4"].Value;
                    double allowed = value / 30.0 * temporalHousing.Value["G4"].Usage.Duration.Days.ToDouble();
                    if (consumption > allowed)
                    {
                        double m = (allowed) / temporalHousing.Value["G4"].Usage.Duration.Days.ToDouble() * 30.0;
                        temporalUsage = temporalHousing.Value["G4"].Usage.Clone(null, null, temporalHousing.Value["G4"].Usage.Karbari, allowed, m, value);
                    }*/

                    freeAmount = 0;// (consumption - allowed);
                }
                InTime timeObj2 = temporalUsage.InTime();
                LinkedListNode<Dictionary<string, Block>> underConstruction = null;
                if (SerialNo != null)
                {
                    var underConstructions = usage.Gaps.Select(new EntryHash(EntryType.Extend, Options.CityId, SerialNo.Value, (int)GapType.UnderConstruction, 0, 0, 0), () =>
                    {
                        Frame<Block> resolved = new Frame<Block>(EntryHash.Empty);
                        Block defaultUnderConstruction = new Block(temporalUsage.StartedAt, 0, 0, 0, "G1", 0);
                        defaultUnderConstruction.SetDuration(temporalUsage.EndedAt);
                        defaultUnderConstruction.SetUsage(temporalUsage);
                        resolved.Add(defaultUnderConstruction);
                        return resolved;
                    }).Extract<LinkedList<Dictionary<string, Block>>>(new Link(timeObj2.From, timeObj2.To, temporalUsage, 0));
                    underConstruction = underConstructions.First;
                }

                while (underConstruction != null)
                {
                    int tariff2 = underConstruction.Value["G1"].Value == 0 ? tariff1 : 24;
                    if (tariff2 == 24)
                        tariffToShow = 24;
                    //if (Options.Tariff == 25)
                    //    tariff = 85;
                    IUsageSubject underConstructionUsage = underConstruction.Value["G1"].Usage;
                    if (new double[] { 43, 91 }.Contains(Options.CategoryUses) && freeAmount > 0)
                    {
                        tariff2 = 85;
                    }
                    InTime familyTimeObj = underConstructionUsage.InTime();
                    LinkedListNode<Dictionary<string, Block>> family = null;
                    if (SerialNo != null)
                    {
                        var families = underConstructionUsage.Gaps.Select(new EntryHash(EntryType.Extend, Options.CityId, SerialNo.Value, (int)GapType.Family, 0, 0, 0), () =>
                        {
                            Frame<Block> resolved = new Frame<Block>(EntryHash.Empty);
                            Block defaultFamily = new Block(underConstructionUsage.StartedAt, 0, 9999999, 1, "F", 0);
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
                        //Water Cost Rate Fetching
                        Frame<Block> Series = new Frame<Block>(EntryHash.Empty);
                        Frame<Block> WCHash = DataSeries.Select(new EntryHash(EntryType.Water, Options.CityId, tariff2, 17, 0, 0, 1)).Transform(new InVolume(familyUsage.MonthlyAverage / family.Value["F"].Value));
                        if (Options.ReadCode == 5)
                        {
                            tariff2 = Options.Tariff == 11 ? (usage.MonthlyAverage > patternLimit ? 110 : Options.Tariff) : Options.Tariff;
                            Frame<Block> WCHash2 = DataSeries.Select(new EntryHash(EntryType.Water, Options.CityId, tariff2, 17, 0, 0, 1)).Transform(new InVolume(familyUsage.MonthlyAverage / family.Value["F"].Value));
                            WCHash2.Items = WCHash2.Items.Select(x => new Block(x.Date, x.FromRange, x.ToRange, x.Value, "17", x.Extra)).ToList();
                            tariffToShow = 11;
                            if (WCHash2.Count() > 0)
                            {
                                WCHash = WCHash2;
                            }
                        }
                        if (Options.ReadCode != 5 && tariff2 != 24 && ((Options.Karbari == 6 && Options.CategoryUses != 409) || (Options.Karbari != 1 && Options.Karbari != 1 && Options.FineCost.Contains(Options.CategoryUses) || (Options.Karbari != 1 && Options.FineCost.IsSpecial(Options.EstateId)))))
                        {
                            Frame<Block> WCHash2 = DataSeries.Select(new EntryHash(EntryType.Water, Options.CityId, 85, 17, 0, 0, 1));
                            tariff2 = 85;
                            if (WCHash2.Count() > 0)
                            {
                                WCHash = WCHash2;
                            }
                        }
                        Frame<Block> CHash = DataSeries.Select(new EntryHash(EntryType.Water, Options.CityId, 0, 14, 0, 0, 1)).Transform(new InVolume(familyUsage.MonthlyAverage / family.Value["F"].Value));
                        //Entry CRate = D[CHash].Slice(cloned);
                        InTime partailTime = familyUsage.InTime();
                        LinkedList<Dictionary<string, Block>> rates = Series.Transform(new MergeFrame(WCHash)).Transform(new MergeFrame(CHash))
                                .Extract<LinkedList<Dictionary<string, Block>>>(new Link(partailTime.From, partailTime.To, familyUsage));
                        LinkedListNode<Dictionary<string, Block>> currentStep = rates.First;
                        int maxLoop = 10;
                        while (currentStep != null)
                        {
                            IUsageSubject currentUsage = currentStep.Value.First().Value.Usage;
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
                            int tariff3 = tariff2;
                            while (illegal != null)
                            {
                                tariff2 = tariff3;
                                int illegalId = 0;
                                illegalId = illegal.Value["I"].Value.ToString().ToInt();
                                IUsageSubject illegalUsage = illegal.Value.First().Value.Usage;
                                double rangeDuration = illegal.Value.First().Value.Duration.Days.ToString().ToDouble();
                                if (rangeDuration <= 0)
                                {
                                    illegal = illegal.Next;
                                    continue;
                                }
                                InTime partailTime3 = illegalUsage.InTime();
                                double cost = currentStep.Value["17"].Value;
                                if (new int[] { 5 }.Contains(tariff2))
                                    cost = rates.Last.Value["17"].Value;
                                double extra = currentStep.Value["17"].Extra;
                                /*if (Options.CityId == 22 && SerialNo != null && SerialNo == 16561)
                                {
                                    cost = 66627;
                                }*/
                                if (Options.CityId == 5 && SerialNo != null && SerialNo == 54671)
                                {
                                    cost = 100000;// 28098;
                                }
                                if (Options.CityId == 22 && SerialNo != null && SerialNo == 16561) // فولاد شادگان از اول سال 1403
                                {
                                    cost = 75000;// 28098;
                                }
                                if (Options.CityId == 16 && SerialNo != null && SerialNo == 8726)
                                {
                                    //cost = 110000;// 28098; sal 1402
                                    cost = 200000;// 28098; sal 1403
                                }
                                if (Options.CityId == 14 && SerialNo != null && SerialNo == 53000)
                                {
                                    cost = 31470;// سال 1402
                                    cost = 35246;// سال 1403 پالایشگاه بهبهان
                                }
                                if (Options.CityId == 28 && SerialNo != null && SerialNo == 42459)
                                {
                                    cost = 610000;// انشعاب موقت نفت
                                }
                                if (Options.CityId == 66 && SerialNo != null && SerialNo == 203)
                                {
                                    cost = 249065;// اشتراک شرکت نفت اهواز
                                }
                                if (illegalId > 0)
                                {
                                    var ILHash = DataSeries.Select(new EntryHash(EntryType.Water, Options.CityId, 5, 17, 0, 0, 1)).Transform(new InVolume(currentUsage.MonthlyAverage)).Extract<LinkedList<Dictionary<string, Block>>>(new Link(partailTime2.From, partailTime2.To, currentUsage, 1));
                                    if (ILHash != null && ILHash.Count > 0)
                                    {
                                        Options.IllegalId = illegalId;
                                        tariff2 = 5;
                                        tariffToShow = 5;
                                        cost = ILHash.First.Value["17"].Value;// .Value["I"].Value;
                                        extra = 0;
                                    }
                                }
                                if (Options.CategoryUses == 409)
                                {
                                    tariff2 = 88;
                                    tariffToShow = 88;
                                    cost = 252165;// 28098;
                                }

                                double priceItem = 0;
                                double cityRate = currentStep.Value["14"].Value;
                                if (tariffToShow == 87)
                                    cityRate = 1;

                                if (Options.Karbari != 1 || new int[] { 5, 24 }.Contains(tariff2))
                                {
                                    cityRate = (cityRate < 1) ? 1 : cityRate;
                                    if (Options.CityId == 28 && SerialNo != null && SerialNo == 42459)
                                    {
                                        cityRate = 1;// انشعاب موقت نفت
                                    }
                                }
                                
                                int cityId = Options.CityId;
                                if (new int[] { 2620, 5947 }.Contains(Options.CycleId))
                                {
                                    cityId = 5;
                                }
                                if (new int[] { 1632 }.Contains(Options.CycleId))
                                {
                                    cityId = 24;
                                }
                                if (new int[] { 2582, 2645 }.Contains(Options.CycleId))
                                {
                                    cityId = 76;
                                }
                                if (new int[] { 6368 }.Contains(Options.CycleId))
                                {
                                    cityId = 49;
                                }
                                
                                if (Options.Karbari == 1)
                                {
                                    var formula = DataSeries.Select(new EntryHash(EntryType.Water, 0, 17, 6, 0, 0, 0)).Extract<LinkedList<Dictionary<string, Block>>>(new Link(partailTime3.From, partailTime3.To, illegalUsage, 1));
                                    var action = formula.First;
                                    while (action != null)
                                    {
                                        IUsageSubject formulaUsage = action.Value.First().Value.Usage;
                                        InTime formatTime = formulaUsage.InTime();
                                        rangeDuration = formulaUsage.Duration.Days.ToDouble();
                                        var version = action.Value["6"].Value.ToString().ToInt();

                                        bool volumeRange = false;
                                        double monthly = formulaUsage.MonthlyAverage / family.Value["F"].Value;
                                        if (DateTime.Parse("2023-03-19 00:00:00.000") <= formatTime.From)
                                            volumeRange = (monthly >= 5 && monthly <= 17.00000001 && tariff2 == 11);
                                        if (DateTime.Parse("2024-09-16 00:00:00.000") <= formatTime.From)
                                            volumeRange = (monthly >= 4 && monthly <= 17.00000001 && tariff2 == 11);
                                        if (new int[] { 11, 110 }.Contains(tariff2) == false || volumeRange)
                                            version = 1;
                                        ApplyCityRate applyCityRate = new ApplyCityRate(DataSeries);

                                        switch (version)
                                        {
                                            case 0:
                                            case 1:
                                                priceItem = applyCityRate.Apply(formulaUsage, cityId, new int[] { 11, 110 }.Contains(tariff2) ? 11 : tariff2, Options.IsVillage, family.Value["F"].Value, (r, cityDuration) =>
                                                {
                                                    return ((cost * monthly - extra) * (cityDuration / 30) * r) * family.Value["F"].Value * (new int[] { 5, 24 }.Contains(tariff2) == false && volumeRange ? 1.15 : 1);
                                                    //priceItem_v2 * patternUsage.Consumption * r * (cityDuration / formulaUsage.Duration.Days.ToDouble());
                                                });
                                                //priceItem = ((cost * monthly - extra) * (rangeDuration / 30) * cityRate) * family.Value["F"].Value * (volumeRange ? 1.15 : 1);

                                                break;
                                            case 2:
                                                var WCHash3 = DataSeries.Select(new EntryHash(EntryType.Water, Options.CityId, 85, 17, 0, 0, 1)).Extract<LinkedList<Dictionary<string, Block>>>(new Link(formatTime.From, formatTime.To, formulaUsage, 0));
                                                var WCRate = WCHash3.First;
                                                while (WCRate != null)
                                                {
                                                    cost = WCRate.Value["17"].Value;
                                                    double priceItem_v2 = 0;
                                                    IUsageSubject patternUsage = WCRate.Value.First().Value.Usage;
                                                    monthly = (patternUsage.MonthlyAverage / family.Value["F"].Value);
                                                    if (0 < patternUsage.MonthlyAverage && monthly <= patternLimit)
                                                    {
                                                        priceItem_v2 = 0.01 * cost * monthly;
                                                    }
                                                    else if (patternLimit < monthly && monthly <= (patternLimit * 3))
                                                    {
                                                        priceItem_v2 = (0.01 * cost * monthly) + (0.02 * cost * (monthly - patternLimit));
                                                    }
                                                    else
                                                    {
                                                        priceItem_v2 = (0.01 * cost * monthly) + (0.03 * cost * (monthly - patternLimit));
                                                    }

                                                    priceItem_v2 = applyCityRate.Apply(patternUsage, cityId, Options.Tariff, Options.IsVillage, family.Value["F"].Value, (r, cityDuration) =>
                                                    {
                                                        return priceItem_v2 * patternUsage.Consumption * r;// *(cityDuration / formulaUsage.Duration.Days.ToDouble());
                                                    });
                                                    //priceItem_v2 = priceItem_v2 * patternUsage.Consumption * cityRate;
                                                    priceItem += priceItem_v2;
                                                    WCRate = WCRate.Next;
                                                }
                                                break;
                                        }
                                        if (Options.IsVillage && Options.Karbari == 1 && new int[] { 5, 24, 87 }.Contains(tariffToShow) == false)
                                            priceItem = priceItem / 2;
                                        price += priceItem;
                                        action = action.Next;
                                    }
                                }
                                else
                                {
                                    ApplyCityRate applyCityRate = new ApplyCityRate(DataSeries);
                                    priceItem = applyCityRate.Apply(illegalUsage, cityId, Options.Tariff, Options.IsVillage, family.Value["F"].Value, (r, cityDuration) =>
                                    {
                                        if(Options.CityId == 66 && SerialNo != null && SerialNo == 203)
                                        {
                                            r = 1;// اشتراک شرکت نفت اهواز
                                        }
                                        if (new int[] { 5 }.Contains(tariff2))
                                        {
                                            r = CHash.LastOrDefault().Value;
                                            cost = DataSeries.Select(new EntryHash(EntryType.Water, Options.CityId, 5, 17, 0, 0, 1)).LastOrDefault().Value;
                                        }
                                        return illegalUsage.Consumption * cost * r * (cityDuration / illegalUsage.Duration.Days.ToDouble());
                                    });
                                    //priceItem = (illegalUsage.Consumption * cost * cityRate);
                                    price += priceItem;
                                }

                                if (underConstruction.Value.First().Value.Value == 1)
                                {
                                    /*|| illegal.Value.First().Value.Value > 0*/
                                    string state = string.Format("{0}:{1}:{2}",
                                       underConstruction.Value.First().Value.Value
                                       , illegal.Value.First().Value.Value
                                       , rangeDuration);
                                    Effect(new Transaction(TransactionSource.WaterCost, TransactionType.Transit, priceItem, state));
                                }
                                /*if (new int[] { 78, 92 }.Contains(Options.CategoryUses))
                                {
                                    Frame<Block> taneshAbi = DataSeries.Select(new EntryHash(EntryType.Water, Options.CityId, 0, 10, 0, 0, 0));
                                    Frame<Block> ZaribCityHash = DataSeries.Select(new EntryHash(EntryType.Water, Options.CityId, 0, 11, 0, 0, 0));
                                    LinkedList<Dictionary<string, Block>> zaribCorrection = DataSeries.Select(new EntryHash(EntryType.Water, Options.CityId, 1, 12, 0, 0, 0))
                                        .Transform(new MergeFrame(taneshAbi))
                                        .Transform(new MergeFrame(ZaribCityHash))
                                        .Extract<LinkedList<Dictionary<string, Block>>>(new Link(partailTime2.From, partailTime2.To, currentUsage, 1));
                                    if (zaribCorrection.First != null)
                                    {
                                        double schoolPattern = currentUsage.Capacity / (zaribCorrection.First.Value["12"].Value * 0.45) * zaribCorrection.First.Value["10"].Value;
                                        double spz = schoolPattern / 30 * currentUsage.Duration.Days.ToDouble();
                                        if (Options.CategoryUses == 92)
                                        {
                                            spz *= 2;
                                        }
                                        if (spz >= currentUsage.Consumption)
                                        {
                                            Effect(new Transaction(TransactionSource.WaterCost, TransactionType.Discount, priceItem, "مخفف الگوی مدارس"));
                                        }
                                    }
                                }*/
                                illegal = illegal.Next;
                            }
                            currentStep = currentStep.Next;
                            maxLoop--;
                            if (maxLoop == 0)
                                break;
                        }
                        family = family.Next;
                    }
                    //End Under Construction


                    underConstruction = underConstruction.Next;
                }

                temporalHousing = temporalHousing.Next;
            }
            if (Options.TedadVahed > 1)
                price *= Options.TedadVahed;

            Effect(new Transaction(TransactionSource.WaterCost, TransactionType.Debit, price));
            if (codes.Contains(Options.CategoryUses) || (Options.CategoryUses == 154 && Options.IsVillage))
            {
                Effect(new Transaction(TransactionSource.WaterCost, TransactionType.Discount, price));
            }
            //if (Options.Tariff != 86)
            Options.Tariff = tariff == 85 ? 25 : tariffToShow;
        }
    }
}