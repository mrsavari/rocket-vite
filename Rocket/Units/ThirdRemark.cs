using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.Core;
using Rocket.States;
using Rocket.Options;

namespace Rocket.Units
{
    public class ThirdRemark: Unit, IObserver
    {
        public ThirdRemark(TimeSeries series, ThirdRemarkOptions options, FineCost fineCost, int? serialNo = null)
            : base()
        {
            Options = options;
            DataSeries = series;
            SerialNo = serialNo;
            FineCost = fineCost;
        }
        public ThirdRemarkOptions Options { get; set; }
        public TimeSeries DataSeries { get; set; }
        public FineCost FineCost { get; set; }
        public int? SerialNo { get; set; }
        public override void Update(object sender, EventArgs e)
        {
            Reset(TransactionSource.ThirdRemark);
            IUsageSubject usage1 = Subject as IUsageSubject;
            InTime partailTime1 = usage1.InTime();
            var formula = DataSeries.Select(new EntryHash(EntryType.Water, 0, 23, 6, 0, 0, 0)).Extract<LinkedList<Dictionary<string, Block>>>(new Link(partailTime1.From, partailTime1.To, usage1, 1));
            var action = formula.First;
            while (action != null)
            {
                IUsageSubject usage = action.Value.First().Value.Usage;
                var version = action.Value["6"].Value.ToString().ToInt();
                switch (version)
                {
                    case 0:
                    case 1:
                        if (Options.Karbari == 1 && Options.IsVillage && Options.CategoryUses == 154)
                            return;
                        if (Options.CityId == 22 && SerialNo != null && SerialNo == 16561)
                        {
                            return;
                        }
            
                        int[] abadan = new int[] { 54622, 54623, 54629, 59276, 59277, 59279, 54628, 54671, 54625, 54626, 54621, 59278, 52689, 52691, 21846, 52690, 40213, 47385, 725, 54690, 62359, 320, 24035, 347, 6638, 16746, 42874, 70917, 70918, 73168, 73172, 73173, 83647, 83648, 100316 };
                        if (Options.Karbari != 1 && (FineCost.Contains(Options.CategoryUses) && (Options.CityId == 5 && SerialNo != null && abadan.Contains(SerialNo.Value)) == false && (Options.CityId == 66 && SerialNo != null && SerialNo.Value == 83) == false && (Options.CityId == 60 && Options.CategoryUses == 69) == false && (Options.CityId == 16 && Options.CategoryUses == 53) == false))
                        {
                            return;
                            /*if (SerialNo == null || (Options.CityId != 60 &&  new int[] { 990, 991, 6069 }.Contains(SerialNo.Value)))
                            {
                                return;
                            }*/
                        }
                        double price = 0;
                        //int[] tankers = { 21, 16, 19,22,49,12,69 };
                        if (Options.IsVillage /*|| (!tankers.Contains(Options.CityId) && Options.Karbari == 22)*/)// || (Options.GetNumber("CityId") != 21 ))
                            return;
                        int[] codes = { 55, 79, 262, 238, 97, 271, 1, 59, 58, 57, 56, 63, 62, 61, 60, 70, 152, 80, 35,82,83,84,156 };
                        int[] tariffs = { 23,7, 9 };
                        if (codes.Contains(Options.CategoryUses) || tariffs.Contains(Options.Tariff))
                            return;
            
                        if (Options.TedadVahed > 1)
                            usage = usage.Clone(null,null,null,usage.Consumption / Options.TedadVahed,null,null);
                        if (Options.CityId == 21)
                        {
                            if (Options.Tariff == 24 || Options.Karbari == 7)
                                return;
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
                        if (Options.CityId == 69 || Options.CityId == 22)
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
                                if (Options.Karbari != 1 && new int[] { 13, 11, 25, 57, 8, 59, 58, 12, 49, 22, 19, 16, 21, 69 }.Contains(Options.CityId) && (underConstruction.Value.First().Value.Value > 0 || illegal.Value.First().Value.Value > 0))
                                {
                                    illegal = illegal.Next;
                                    continue;
                                }
                                LinkedListNode<Dictionary<string, Block>> family = null;
                                if (SerialNo != null)
                                {
                                    var families = illegalUsage.Gaps.Select(new EntryHash(EntryType.Extend, Options.CityId, SerialNo.Value, (int)GapType.Family, 0, 0, 0), () =>
                                    {
                                        Frame<Block> resolved = new Frame<Block>(EntryHash.Empty);
                                        Block defaultFamily = new Block(illegalUsage.StartedAt, 0, 0, 1, "F", 0);
                                        defaultFamily.SetDuration(illegalUsage.EndedAt);
                                        defaultFamily.SetUsage(illegalUsage);
                                        resolved.Add(defaultFamily);
                                        return resolved;
                                    }).Extract<LinkedList<Dictionary<string, Block>>>(new Link(timeObj.From, timeObj.To, illegalUsage, 1));
                                    family = families.First;
                                }

                                while (family != null)
                                {
                                    IUsageSubject familyUsage = family.Value["F"].Usage;
                                    double monthly = familyUsage.MonthlyAverage;
                                    if (monthly == 0)
                                    {
                                        family = family.Next;
                                        continue;
                                    }
                                    InTime partialObj = familyUsage.InTime();
                                    Frame<Block> Series = new Frame<Block>(EntryHash.Empty);
                                    Frame<Block> TRHash = DataSeries.Select(new EntryHash(EntryType.Water, Options.CityId, Options.Karbari, 23, 0, 0, 1)).Transform(new InVolume(monthly / family.Value["F"].Value));
                                    //هفتکل علکی
                                    DateTime startPoint = DateTime.Parse("2023-02-13 00:00:00.000");
                                    if ((startPoint < usage.EndedAt) && Options.CityId == 60 && (Options.CategoryUses == 328 || Options.CategoryUses == 69 || new int[] { 251, 252, 253, 390, 249, 250, 408, 248, 255 }.Contains(Options.CategoryUses)))
                                    {
                                        if (Options.CategoryUses == 328)
                                        {
                                            TRHash.Items.RemoveAll(x => x.Date >= DateTime.Parse("2023-02-13 00:00:00.000"));
                                            TRHash.Items.Add(new Block(DateTime.Parse("2023-02-13 00:00:00.000"), 0, 0, 4000, "23", 0));
                                            TRHash.Items.Add(new Block(DateTime.Parse("2024-02-13 00:00:00.000"), 0, 0, 0, "23", 0));
                                        }
                                        if (Options.CategoryUses == 69)
                                        {
                                            TRHash.Items.RemoveAll(x => x.Date >= DateTime.Parse("2023-02-13 00:00:00.000"));
                                            TRHash.Items.Add(new Block(DateTime.Parse("2023-02-13 00:00:00.000"), 0, 0, 5000, "23", 0));
                                            TRHash.Items.Add(new Block(DateTime.Parse("2024-02-13 00:00:00.000"), 0, 0, 0, "23", 0));
                                        }

                                        //مرغداری و غیره
                                        //if (usage.StartedAt >= DateTime.Parse("2023-02-13 00:00:00.000"))
                                        //{
                                        if (new int[] { 251, 252, 253, 390, 249, 250, 408, 248, 255 }.Contains(Options.CategoryUses))
                                        {
                                            TRHash.Items.RemoveAll(x => x.Date >= DateTime.Parse("2023-02-13 00:00:00.000"));
                                            TRHash.Items.Add(new Block(DateTime.Parse("2023-02-13 00:00:00.000"), 0, 0, 15000, "23", 0));
                                            TRHash.Items.Add(new Block(DateTime.Parse("2024-02-13 00:00:00.000"), 0, 0, 0, "23", 0));
                                        }
                                        if (Options.CategoryUses == 328)
                                        {
                                            TRHash.Items.RemoveAll(x => x.Date >= DateTime.Parse("2023-02-13 00:00:00.000"));
                                            TRHash.Items.Add(new Block(DateTime.Parse("2023-02-13 00:00:00.000"), 0, 0, 70000, "23", 0));
                                            TRHash.Items.Add(new Block(DateTime.Parse("2024-02-13 00:00:00.000"), 0, 0, 0, "23", 0));
                                        }
                                        if (Options.CategoryUses == 69)
                                        {
                                            TRHash.Items.RemoveAll(x => x.Date >= DateTime.Parse("2023-02-13 00:00:00.000"));
                                            TRHash.Items.Add(new Block(DateTime.Parse("2023-02-13 00:00:00.000"), 0, 0, 80000, "23", 0));
                                            TRHash.Items.Add(new Block(DateTime.Parse("2024-02-13 00:00:00.000"), 0, 0, 0, "23", 0));
                                        }
                                        //}
                                        /*if (usage.EndedAt < DateTime.Parse("2024-02-13 00:00:00.000"))
                                        {
                                            price = usage.Consumption * someRate;
                                            Effect(new Transaction(TransactionSource.ThirdRemark, TransactionType.Debit, price));
                                            return;
                                        }*/
                                    }
                                    if (Options.CityId == 66 && SerialNo == 70)
                                    {
                                        TRHash.Items.RemoveAll(x => x.Date >= DateTime.Parse("2023-02-13 00:00:00.000"));
                                        TRHash.Items.Add(new Block(DateTime.Parse("2023-02-13 00:00:00.000"), 0, 0, 5000, "23", 0));
                                    }
                                    if (SerialNo != null && (Options.CityId == 5 && abadan.Contains(SerialNo.Value)) || (Options.CityId == 66 && SerialNo != null && SerialNo.Value == 83))
                                    {
                                        TRHash.Items.RemoveAll(x => x.Date == DateTime.Parse("2023-05-01 00:00:00.000"));
                                        TRHash.Items.Add(new Block(DateTime.Parse("2023-05-01 00:00:00.000"), 0, 0, 60000, "23", 0));
                                        TRHash.Items.Add(new Block(DateTime.Parse("2024-04-30 00:00:00.000"), 0, 0, 0, "23", 0));
                                    }
                                    if (Options.CityId == 14)
                                    {
                                        if (SerialNo != null && SerialNo.Value == 53000) {
                                            TRHash.Items.Clear();
                                            TRHash.Items.Add(new Block(DateTime.Parse("2023-03-15 00:00:00.000"), 0, 0, 70000, "23", 0));
                                            TRHash.Items.Add(new Block(DateTime.Parse("2024-03-15 00:00:00.000"), 0, 0, 0, "23", 0));
                                        }
                                        else {
                                            int karbari = (underConstruction.Value.First().Value.Value > 0 || illegal.Value.First().Value.Value > 0)? 7: Options.Karbari;
                                            TRHash = DataSeries.Select(new EntryHash(EntryType.Water, Options.CityId, karbari, 23, 0, 0, 1));
                                        }
                                    }
                                    //Entry BPRate = D[TRHash].Slice(familyUsage);

                                    LinkedList<Dictionary<string, Block>> rates =
                                        Series.Transform(new MergeFrame(TRHash)).Extract<LinkedList<Dictionary<string, Block>>>(new Link(partialObj.From, partialObj.To, familyUsage));// (D.GetFamily(familyUsage) + BPRate).GetBlocks(familyUsage);
                                    LinkedListNode<Dictionary<string, Block>> currentStep = rates.First;
                                    int maxLoop = 10;
                                    while (currentStep != null)
                                    {
                                        double rangeDuration = double.Parse(currentStep.Value.First().Value.Duration.Days.ToString());
                                        if (rangeDuration < 1)
                                            break;
                                        double amount = 0.0;
                                        if (Options.ReadCode == 5)
                                        {
                                            amount = familyUsage.MonthlyAverage / family.Value["F"].Value;
                                            familyUsage = familyUsage.Clone(currentStep.Value.First().Value.Usage.StartedAt, currentStep.Value.First().Value.Usage.EndedAt, null, amount, null, null);
                                        }
                                        else
                                        {
                                            amount = familyUsage.Consumption / family.Value["F"].Value;
                                            if (family.Value["F"].Value > 1)
                                                amount = familyUsage.MonthlyAverage / family.Value["F"].Value;
                                        }
                            
                                        price += ((rangeDuration / 30.0) * currentStep.Value["23"].Extra) * family.Value["F"].Value;
                                        price += ((amount * rangeDuration / familyUsage.Duration.Days.ToDouble()) * currentStep.Value["23"].Value) * family.Value["F"].Value;
                                        currentStep = currentStep.Next;
                                        maxLoop--;
                                        if (maxLoop == 0)
                                            break;

                                    }
                                    family = family.Next;
                                }

                                illegal = illegal.Next;
                            }
                            underConstruction = underConstruction.Next;
                        }
            
                        if (Options.TedadVahed > 1)
                            price *= Options.TedadVahed;
                        Effect(new Transaction(TransactionSource.ThirdRemark,TransactionType.Debit,price));
                        break;
                    case 2:
                        Effect(new Transaction(TransactionSource.ThirdRemark, TransactionType.Debit, 0));
                        break;
                }
                action = action.Next;
            }
            
        }
        
    }
}