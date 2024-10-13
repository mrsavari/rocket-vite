using Rocket.Core;
using Rocket.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rocket.Branch
{
    public class ExtraPipe : Unit, IObserver
    {
        public ExtraPipe(TimeSeries series, ExtraDigOptions options)
            : base()
        {
            Options = options;
            DataSeries = series;
        }
        public ExtraDigOptions Options { get; set; }
        public TimeSeries DataSeries { get; set; }

        public override void Update(object sender, EventArgs e)
        {
            Reset(TransactionSource.ExtraPipe);
            IUsageSubject usage = Subject as IUsageSubject;
            /*
            if (Options.Pipe) OutSide Checking
            {
                return;
            }
            if (type == CostType.Sewage && Options.Get("KindBranchId") == "2")
                return;
            if (type == CostType.Water && Options.Get("KindBranchId") == "3")
                return;
            */
            double distance = (Options.Type == CalculationType.Water) ? Options.DistanceToMeter : Options.DistanceToSifon;
            if (distance > 0)
            {
                int costTypeId = 0;
                if (Options.Type == CalculationType.Water)
                    costTypeId = 21;
                else
                    costTypeId = 31;
                InTime timeObj = usage.InTime();
                LinkedList<Dictionary<string, Block>> rates = DataSeries.Select(new EntryHash(EntryType.Branch, Options.CityId, 1, costTypeId, 0, Options.WaterDiameterId, 0))
                                                               .Extract<LinkedList<Dictionary<string, Block>>>(new Link(timeObj.From, timeObj.To, null));
                if (rates.Count > 0)
                {
                    Effect(new Transaction(TransactionSource.ExtraPipe, TransactionType.Debit, rates.First.Value[costTypeId.ToString()].Value * distance));
                }
            }

        }
    }
}
