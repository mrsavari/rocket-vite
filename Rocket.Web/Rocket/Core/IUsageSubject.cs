using Rocket.States;
using System;
using System.Collections.Generic;

namespace Rocket.Core
{
    public interface IUsageSubject: ISubject
    {
        int Karbari {get;set;}
        double Consumption {get;set;}
        double MonthlyAverage {get;set;}
        double Capacity {get;set;}

        DateTime StartedAt {get;set;}
        DateTime EndedAt {get;set;}
        TimeSpan Duration {get;}
        UsageState State { get; set; }
        GapSeries Gaps { get; set; }
        UsageSubject SetTime(DateTime? startedAt,DateTime? endedAt);
        InTime InTime();
        UsageSubject Clone(DateTime? startedAt,DateTime? endedAt,int? karbari,double? consumption,double? monthlyAverage,double? capacity);
    }
}