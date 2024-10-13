using System;
using Rocket.Core;

namespace Rocket.States
{
    [Serializable]
    public class OverUsageState : UsageState
    {

        public OverUsageState(UsageState state,IUsageSubject usage){
            this.Usage = state.Usage;
            this.consumption = usage.Consumption;
            this.maximum = (usage.Capacity / 30 * usage.Duration.Days.ToDouble());
            pattern = 16.0;
            this.extra = usage.Consumption - maximum;
        }

        public override double GetExtraUsage(){
            return this.extra;
        }

        public override void Update(object sender, EventArgs e)
        {
            IUsageSubject usage = Subject as IUsageSubject;
            if (usage != null)
            {
                if (usage.Consumption <= maximum)
                {
                    Usage.State = new NormalUsageState(usage);
                    Usage.State.Subject = usage;
                }
            }
        }
    }
}