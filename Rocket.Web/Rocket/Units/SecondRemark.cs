using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.Core;
using Rocket.Options;

namespace Rocket.Units
{
    public class SecondRemark: Unit, IObserver
    {
        public SecondRemark(TimeSeries series, SecondRemarkOptions options,FineCost fineCost, int? serialNo = null)
            : base()
        {
            Options = options;
            DataSeries = series;
            SerialNo = serialNo;
            FineCost = fineCost;
        }
        public SecondRemarkOptions Options { get; set; }
        public TimeSeries DataSeries { get; set; }
        public FineCost FineCost { get; set; }
        public int? SerialNo { get; set; }

        public override void Update(object sender, EventArgs e)
        {
            Reset(TransactionSource.SecondRemark);
            IUsageSubject usage1 = Subject as IUsageSubject;
            InTime partailTime1 = usage1.InTime();
            var formula = DataSeries.Select(new EntryHash(EntryType.Water, 0, 22, 6, 0, 0, 0)).Extract<LinkedList<Dictionary<string, Block>>>(new Link(partailTime1.From, partailTime1.To, usage1, 1));
            var action = formula.First;
            while (action != null)
            {
                IUsageSubject usage = action.Value.First().Value.Usage;
                var version = action.Value["6"].Value.ToString().ToInt();
                switch (version)
                {
                    case 0:
                    case 1:
                        int[] tariffs = { 7, 9, 22,23 };
                        if (Options.IsVillage || Options.KindBranch != 2 || tariffs.Contains(Options.Tariff))
                            return;
                        if (Options.Karbari == 1 && Options.IsVillage && Options.CategoryUses == 154)
                            return;
                        if (Options.CityId == 22 && SerialNo != null && SerialNo == 16561)
                        {
                            return;
                        }
                        if(Options.Karbari != 1 && FineCost.Contains(Options.CategoryUses))
                            return;

                        IHasTransactions trans = Subject as IHasTransactions;
                        double rate = (trans.Balance(TransactionSource.WaterCost) + trans.Balance(TransactionSource.FreeWater) + trans.Balance(TransactionSource.Seasonal) + trans.Balance(TransactionSource.WaterPattern) + trans.Balance(TransactionSource.BrokenPipe)) / double.Parse(usage1.Duration.Days.ToString());
                        InTime timeObj = usage.InTime();
                        Frame<Block> Series = new Frame<Block>(EntryHash.Empty);
                        Frame<Block> SRHash = DataSeries.Select(new EntryHash(EntryType.Water, Options.CityId, 0, 22, 0, 0, 1)).Transform(new InVolume(usage.MonthlyAverage));
                        //Entry SRRate = D[SRHash].Slice(options);
                        LinkedList<Dictionary<string, Block>> rates = Series.Transform(new MergeFrame(SRHash))
                                .Extract<LinkedList<Dictionary<string, Block>>>(new Link(timeObj.From, timeObj.To, usage));

                        //LinkedList<Dictionary<string, Block>> rates = SRRate.GetBlocks(options);
                        LinkedListNode<Dictionary<string, Block>> currentStep = rates.First;
                        int maxLoop = 10;
                        double price =0;
                        while (currentStep != null)
                        {
                            double rangeDuration = currentStep.Value.First().Value.Duration.Days.ToString().ToDouble();
                            price += rate * rangeDuration * currentStep.Value["22"].Value;
                            currentStep = currentStep.Next;
                            maxLoop--;
                            if (maxLoop == 0)
                                break;
                        }
                        Effect(new Transaction(TransactionSource.SecondRemark,TransactionType.Debit,price));
                        int[] codes = new int[] { 55, 79, 262, 97, 238, 271 };
                        if (codes.Contains(Options.CategoryUses))
                        {
                            Effect(new Transaction(TransactionSource.SecondRemark,TransactionType.Discount,price));
                        }
                        break;
                   case 2:
                        Effect(new Transaction(TransactionSource.SecondRemark, TransactionType.Debit, 0));
                        break;
                }
                action = action.Next;
            }
            
        }
        
    }
}