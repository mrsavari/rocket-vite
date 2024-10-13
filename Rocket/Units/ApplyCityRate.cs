using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.Core;
using Rocket.Options;

namespace Rocket.Units
{
    public class ApplyCityRate
    {
        public ApplyCityRate(TimeSeries series)
        {
            DataSeries = series;
        }
        public TimeSeries DataSeries { get; set; }

        public double Apply(IUsageSubject usage, int cityId,int tariffId,bool isVillage, double familyCount,Func<double,double,double> apply)
        {
            double result = 0;
            InTime partailTime = usage.InTime();
            var formula = DataSeries.Select(new EntryHash(EntryType.Water, 0, 14, 6, 0, 0, 0)).Extract<LinkedList<Dictionary<string, Block>>>(new Link(partailTime.From, partailTime.To, usage, 1));
            var action = formula.First;
            while (action != null)
            {
                IUsageSubject formulaUsage = action.Value.First().Value.Usage;
                InTime formatTime = formulaUsage.InTime();
                double rangeDuration = formulaUsage.Duration.Days.ToDouble();
                var version = action.Value["6"].Value.ToString().ToInt();
                if (new int[] { 5 }.Contains(tariffId))
                    version = 2;
                version = isVillage ? 1 : version;
                switch (version)
                {
                    case 0:
                    case 1:
                        var CityFragmentV1 = DataSeries.Select(new EntryHash(EntryType.Water, cityId, 0, 14, 0, 0, 1))
                                                       .Transform(new InVolume(formulaUsage.MonthlyAverage / familyCount))
                                                       .Extract<LinkedList<Dictionary<string, Block>>>(new Link(formatTime.From, formatTime.To, formulaUsage, 1));
                        var v1 = CityFragmentV1.First;
                        while (v1 != null)
                        {
                            double ciytRate = v1.Value["14"].Value;
                            ciytRate = tariffId != 11 && ciytRate < 1 ? 1 : ciytRate;
                            ciytRate = tariffId == 87 ? 1 : ciytRate;
                            IUsageSubject v1Usage = v1.Value.First().Value.Usage;
                            double item = apply(ciytRate, v1Usage.Duration.Days.ToDouble());
                            result += item;
                            v1 = v1.Next;
                        }
                    break;
                    case 2:
                    var CityFragmentV2 = DataSeries.Select(new EntryHash(EntryType.Water, cityId, tariffId == 11 ? 11 : 0, 14, 0, isVillage ? 1 : 2, 1))
                                                       .Transform(new InVolume(formulaUsage.MonthlyAverage / familyCount))
                                                       .Extract<LinkedList<Dictionary<string, Block>>>(new Link(formatTime.From, formatTime.To, formulaUsage, 1));
                        var v2 = CityFragmentV2.First;
                        while (v2 != null)
                        {
                            IUsageSubject v2Usage = v2.Value.First().Value.Usage;
                            double item = apply(v2.Value["14"].Value, v2Usage.Duration.Days.ToDouble());
                            result += item;
                            v2 = v2.Next;
                        }
                        break;;
                }
                action = action.Next;
            }
            return result;
        }
    }
}