using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.Core;
using Rocket.States;
using Rocket.Options;

namespace Rocket.Branch
{
    public class ExtraDig : Unit, IObserver
    {
        public ExtraDig(TimeSeries series, ExtraDigOptions options)
            : base()
        {
            Options = options;
            DataSeries = series;
        }
        public ExtraDigOptions Options { get; set; }
        public TimeSeries DataSeries { get; set; }

        public override void Update(object sender, EventArgs e)
        {
            Reset(TransactionSource.ExtraDig);
            IUsageSubject usage = Subject as IUsageSubject;
            IHasTransactions ts = Subject as IHasTransactions;
            double baseCost = 0;
            double SifonEzafe = 1;
            if (Options.Type == CalculationType.Sewage && Options.SifonShared)
            {
                SifonEzafe = Options.SifonEzafeInput / Options.EstateCount;
                baseCost = ts.Balance(TransactionSource.Dig);
            }
            /*  OutSide Checking
            if (Options.Type == CalculationType.Sewage && Options.KindBranch == 2)
                return;
            if (Options.Type == CalculationType.Water && Options.KindBranch == 3)
                return;
            */



            double distance = (Options.Type == CalculationType.Water) ? Options.DistanceToMeter : Options.DistanceToSifon;
            if (distance > 0)
            {
                int costTypeId = 0;
                int phase2 = 0;

                if (Options.Type == CalculationType.Water)
                {
                    costTypeId = 23;
                    phase2 = Options.WaterDiameterId;
                }
                else
                {
                    costTypeId = 33;
                    phase2 = Options.SewageDiameterId;
                }
                InTime timeObj = usage.InTime();
                LinkedList<Dictionary<string, Block>> rates = DataSeries.Select(new EntryHash(EntryType.Branch, Options.CityId, 1, costTypeId, 0, phase2, 0))
                                                               .Extract<LinkedList<Dictionary<string, Block>>>(new Link(timeObj.From, timeObj.To, null));
                LinkedListNode<Dictionary<string, Block>> currentStep = rates.First;
                while (currentStep != null)
                {
                    Effect(new Transaction(TransactionSource.ExtraDig, TransactionType.Debit, currentStep.Value[costTypeId.ToString()].Value * (distance) * SifonEzafe));
                    break;
                }
            }

        }
    }
}
