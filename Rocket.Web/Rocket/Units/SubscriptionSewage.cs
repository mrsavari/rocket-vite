using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.Core;
using Rocket.Options;

namespace Rocket.Units
{
    public class SubscriptionSewage: Unit, IObserver
    {
        public SubscriptionSewage(TimeSeries series,SubscriptionSewageOptions options):base(){
            Options = options;
            DataSeries = series;
        }
        public SubscriptionSewageOptions Options { get; set; }
        public TimeSeries DataSeries { get; set; }

        public override void Update(object sender, EventArgs e)
        {
            Reset(TransactionSource.SubscriptionSewage);
            IUsageSubject usage = Subject as IUsageSubject;
            if (Options.KindBranch == 2 || Options.Karbari == 22 || new int[] { 22, 23 }.ToList<int>().Contains(Options.Tariff) || Options.CategoryUses == 70 || Options.CategoryUses == 77)
                return;
            InTime timeObj = usage.InTime();
            Frame<Block> Series = new Frame<Block>(EntryHash.Empty);
            Frame<Block> SHash = DataSeries.Select(new EntryHash(EntryType.Water, Options.CityId, Options.Tariff, 21, 0, 0, 1));
            //Entry SRate = D[SHash].Slice(options,false);
            LinkedList<Dictionary<string, Block>> rates = Series.Transform(new MergeFrame(SHash))
                                                                .Extract<LinkedList<Dictionary<string, Block>>>(new Link(timeObj.From, timeObj.To,usage));
            LinkedListNode<Dictionary<string, Block>> currentStep = rates.First;
            double price = 0;
            int maxLoop = 10;
            while (currentStep != null)
            {
                double rangeDuration = double.Parse(currentStep.Value.First().Value.Duration.Days.ToString());
                price += (currentStep.Value["21"].Extra / 30) * rangeDuration;
                currentStep = currentStep.Next;
                maxLoop--;
                if (maxLoop == 0)
                    break;
            }
            Effect(new Transaction(TransactionSource.SubscriptionSewage,TransactionType.Debit,price));
            if (Options.Karbari == 7){
                Effect(new Transaction(TransactionSource.SubscriptionSewage,TransactionType.Debit,(Balance * 2) - Balance,"کاربری موقت"));
            }
            if (Options.TedadVahed > 1)
            {
                Effect(new Transaction(TransactionSource.SubscriptionSewage,TransactionType.Debit,(Balance * Options.TedadVahed) - Balance,"اعمال تعداد واحد"));
            }
        }
        
    }
}