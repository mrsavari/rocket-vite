using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.Core;
using Rocket.States;
using Rocket.Options;

namespace Rocket.Branch
{
    public class Building : Unit, IObserver
    {
        public Building(TimeSeries series, BuildingOptions options)
            : base()
        {
            Options = options;
            DataSeries = series;
        }
        public BuildingOptions Options { get; set; }
        public TimeSeries DataSeries { get; set; }

        public override void Update(object sender, EventArgs e)
        {
            Reset(TransactionSource.Building);
            IUsageSubject usage = Subject as IUsageSubject;
            InTime timeObj = usage.InTime();
            LinkedList<Dictionary<string, Block>> rates = DataSeries.Select(new EntryHash(EntryType.Branch, Options.CityId, 0, 14, 0, 0, 0))
                                                           .Extract<LinkedList<Dictionary<string, Block>>>(new Link(timeObj.From, timeObj.To, null));
            double CWP = 1;
            if (rates.Count > 0)
            {
                CWP = rates.First.Value["14"].Value;
            }
            CWP = (CWP > 1) ? CWP : 1;
            double buildCost = (Options.Infrastructure * Convert.ToDouble(Options.DamageCost) * CWP);
            Effect(new Transaction(TransactionSource.Building, TransactionType.Debit, buildCost));

            //AUTO TAXTING
            rates = DataSeries.Select(new EntryHash(EntryType.Branch, Options.CityId, 1, 19, 0, 0, 0))
                                                       .Extract<LinkedList<Dictionary<string, Block>>>(new Link(timeObj.From, timeObj.To, null));
            double price = buildCost * rates.First.Value["19"].Value;
            Effect(new Transaction(TransactionSource.Building, TransactionType.Debit, price));
            double p = 0;
            if (Options.StructureKind == 21)
                p = 2;

            if (Options.StructureKind == 8)
                p = 1.5;

            if (Options.StructureKind == 20)
                p = 1;

            if (DateTime.Now > DateTime.Parse("2022-10-25 00:00:00.000"))
                p = 2;

            if (!Options.IsVillage)
                Effect(new Transaction(TransactionSource.Building, TransactionType.Debit, Options.Infrastructure * 200, Options.DamageTitle));           

        }
    }
}
