using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.Core;
using Rocket.States;
using Rocket.Options;

namespace Rocket.Branch
{
    public class Dig : Unit, IObserver
    {
        public Dig(TimeSeries series, DigOptions options)
            : base()
        {
            Options = options;
            DataSeries = series;
        }
        public DigOptions Options { get; set; }
        public TimeSeries DataSeries { get; set; }

        public override void Update(object sender, EventArgs e)
        {
            Reset(TransactionSource.ThirdRemarkFA);
            IUsageSubject usage = Subject as IUsageSubject;
            double SifonEzafe = 1;

            if (Options.Type == CalculationType.Sewage && Options.SifonShared)
            {
                SifonEzafe = Options.SifonEzafeInput / Options.EstateCount;
            }
            /*  OutSide Checking
            if (Options.Type == CalculationType.Sewage && Options.KindBranch == 2)
                return;
            if (Options.Type == CalculationType.Water && Options.KindBranch == 3)
                return;
             */
            int costTypeId = 0;
            int phase2 = 0;
            if (Options.Type == CalculationType.Water)
            {
                costTypeId = 22;
                phase2 = Options.WaterDiameterId;
            }
            else
            {
                costTypeId = 32;
                phase2 = Options.SewageDiameterId;
            }
            InTime timeObj = usage.InTime();
            LinkedList<Dictionary<string, Block>> rates = DataSeries.Select(new EntryHash(EntryType.Branch, Options.CityId, 1, costTypeId, 0, phase2, 0))
                                                           .Extract<LinkedList<Dictionary<string, Block>>>(new Link(timeObj.From, timeObj.To, null));
            LinkedListNode<Dictionary<string, Block>> currentStep = rates.First;
            while (currentStep != null)
            {
                Effect(new Transaction(TransactionSource.Dig, TransactionType.Debit, currentStep.Value[costTypeId.ToString()].Value * SifonEzafe));
                break;
            }

        }
    }
}
