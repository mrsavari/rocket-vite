using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.Core;
using Rocket.States;
using Rocket.Options;

namespace Rocket.Branch
{
    public class ThirdRemarkFA: Unit, IObserver
    {
        public ThirdRemarkFA(TimeSeries series, ThirdRemarkFAOptions options)
            : base()
        {
            Options = options;
            DataSeries = series;
        }
        public ThirdRemarkFAOptions Options { get; set; }
        public TimeSeries DataSeries { get; set; }

        public override void Update(object sender, EventArgs e)
        {
            Reset(TransactionSource.ThirdRemarkFA);
            IUsageSubject usage = Subject as IUsageSubject;
            if (Options.Tariff == 23)
                return;
            int costTypeId = 0;
            double zarib = 0;
            if (Options.Karbari == 1)
            {
                costTypeId = 3;
                zarib = Options.TedadVahed;
            }
            else
            {
                costTypeId = 4;
                zarib = Options.Capacity * Options.TedadVahed;
            }
            InTime timeObj = usage.InTime();
            Frame<Block> TRFAHash = DataSeries.Select(new EntryHash(EntryType.Branch, Options.CityId, Options.Karbari, costTypeId, Options.AreaId, 0, 0));
            if (TRFAHash.Hash == EntryHash.Empty)
            {
                TRFAHash = DataSeries.Select(new EntryHash(EntryType.Branch, Options.CityId, 1000, costTypeId, Options.AreaId, 0, 0));
            }
            LinkedList<Dictionary<string, Block>> rates = TRFAHash.Extract<LinkedList<Dictionary<string, Block>>>(new Link(timeObj.From, timeObj.To, null));
            LinkedListNode<Dictionary<string, Block>> currentStep = rates.First;
            if (currentStep != null)
            {
                double price = currentStep.Value[costTypeId.ToString()].Value * (zarib) + currentStep.Value[costTypeId.ToString()].Extra;
                Effect(new Transaction(TransactionSource.ThirdRemarkFA, TransactionType.Debit, price));
            }

            double DiscountFactor = Options.TakhfifENFA;
            if (DiscountFactor > 0 && this.Balance > 0)
            {
                if (DiscountFactor >= 100)
                {
                    Effect(new Transaction(TransactionSource.ThirdRemarkFA, TransactionType.Discount, this.Balance));
                }
                else
                {
                    Effect(new Transaction(TransactionSource.ThirdRemarkFA, TransactionType.Discount, this.Balance * (DiscountFactor / 100)));
                }
            }

        }
        
    }
}