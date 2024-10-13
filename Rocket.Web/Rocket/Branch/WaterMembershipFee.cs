using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.Core;
using Rocket.States;
using Rocket.Options;

namespace Rocket.Branch
{
    public class WaterMembershipFee: Unit, IObserver
    {
        public WaterMembershipFee(TimeSeries series, WaterMembershipOptions options)
            : base()
        {
            Options = options;
            DataSeries = series;
        }
        public WaterMembershipOptions Options { get; set; }
        public TimeSeries DataSeries { get; set; }

        public override void Update(object sender, EventArgs e)
        {
            Reset(TransactionSource.WaterMembershipFee);
            IUsageSubject usage = Subject as IUsageSubject;
            if (Options.HasWater || Options.Tariff == 23 || Options.KindBranch == 3)
                return;

            InTime timeObj = usage.InTime();
            Frame<Block> Series = new Frame<Block>(EntryHash.Empty);
            if (Options.CategoryUses == 155)
            {
                Frame<Block> TRABHash = DataSeries.Select(new EntryHash(EntryType.Branch, Options.CityId, 1, 24, 0, Options.WaterDiameterId, 0));
                LinkedList<Dictionary<string, Block>> rates = TRABHash.Extract<LinkedList<Dictionary<string, Block>>>(new Link(timeObj.From, timeObj.To, null));
                if (rates.Count > 0)
                {
                    if (Options.IsVillage && Options.Karbari == 1 && Options.Tariff != 24)
                        Effect(new Transaction(TransactionSource.WaterMembershipFee, TransactionType.Debit, rates.First.Value["24"].Value / 2));
                    else
                        Effect(new Transaction(TransactionSource.WaterMembershipFee, TransactionType.Debit, rates.First.Value["24"].Value));
                }
            }
            else
            {
                int tariff = Options.Tariff;
                if (Options.Karbari == 1)
                {
                    Frame<Block> WMFHash = DataSeries.Select(new EntryHash(EntryType.Branch, Options.CityId, 1, 13, 0, 0, 0));
                    LinkedList<Dictionary<string, Block>> rates = WMFHash.Extract<LinkedList<Dictionary<string, Block>>>(new Link(timeObj.From, timeObj.To, null));
                    if (rates.Count > 0)
                    {
                        double price = 0;
                        LinkedListNode<Dictionary<string, Block>> step = rates.First;
                        while (step != null)
                        {
                            price += step.Value["13"].Value;
                            break;
                        }
                        if (Options.IsVillage && Options.Karbari==1 && Options.Tariff != 24)
                            price = price / 2;
                        Effect(new Transaction(TransactionSource.WaterMembershipFee, TransactionType.Debit, price * Options.TedadVahed));
                    }
                }
                else
                {
                    double result = 0;
                    try
                    {
                        Frame<Block> RateHash = DataSeries.Select(new EntryHash(EntryType.Branch, Options.CityId, 1, 16, 0, 0, 0));
                        Frame<Block> ZoneHash = DataSeries.Select(new EntryHash(EntryType.Branch, Options.CityId, 1, 12, 0, 0, 0));
                        Frame<Block> ZaribCityHash = DataSeries.Select(new EntryHash(EntryType.Branch, Options.CityId, 1, 11, 0, 0, 0));
                        Series = new Frame<Block>(EntryHash.Empty);
                        LinkedList<Dictionary<string, Block>> rates = Series.Transform(new MergeFrame(RateHash))
                                                                            .Transform(new MergeFrame(ZoneHash))
                                                                            .Transform(new MergeFrame(ZaribCityHash))
                                                                            .Extract<LinkedList<Dictionary<string, Block>>>(new Link(timeObj.From, timeObj.To, null));
                        double capacity = Options.Capacity;
                        if (rates.Count > 0)
                        {
                            LinkedListNode<Dictionary<string, Block>> step = rates.First;
                            result = step.Value["16"].Value * step.Value["11"].Value * capacity;
                        }
                    }
                    catch { }
                    try
                    {
                        if (tariff == 13 && Options.ChangeCapacity == false)
                        {
                            Frame<Block> ExHash = DataSeries.Select(new EntryHash(EntryType.Branch, Options.CityId, 1, 15, 0, 0, 0));
                            LinkedList<Dictionary<string, Block>> exRate = ExHash.Extract<LinkedList<Dictionary<string, Block>>>(new Link(timeObj.From, timeObj.To, null));
                            if (exRate.Count > 0)
                            {
                                LinkedListNode<Dictionary<string, Block>> step = exRate.First;
                                result += step.Value["15"].Value;
                            }
                        }
                    }
                    catch { }
                    if (Options.IsVillage && Options.Karbari == 1)
                        result = result / 2;
                    Effect(new Transaction(TransactionSource.WaterMembershipFee, TransactionType.Debit, result * Options.TedadVahed));
                }
            }

            if (Options.TakhfifENAB > 0 && this.Balance > 0)
            {
                if (Options.TakhfifENAB >= 100)
                {
                    Effect(new Transaction(TransactionSource.WaterMembershipFee, TransactionType.Discount, this.Balance));
                }
                else
                {
                    Effect(new Transaction(TransactionSource.WaterMembershipFee, TransactionType.Discount, this.Balance * (Options.TakhfifENAB / 100)));
                }
            }

        }
        
    }
}