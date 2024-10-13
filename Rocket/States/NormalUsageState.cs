using System;
using Rocket.Core;

namespace Rocket.States
{
    [Serializable]
    public class NormalUsageState : UsageState
    {

        public NormalUsageState(UsageState state){
            this.Usage = state.Usage;
            this.consumption = state.Consumption;
            extra = 0;
        }

        public NormalUsageState(IUsageSubject usage)
        {
            Consumption = usage.Consumption;
            maximum = (usage.Capacity / 30 * usage.Duration.Days.ToDouble());
            extra = 0;
            this.Usage = usage;
        }

        public override double GetExtraUsage()
        {
            return 0;
        }

        public override void Update(object sender, EventArgs e)
        {
            IUsageSubject usage = Subject as IUsageSubject;
            if (usage != null)
            {
                if (usage.Consumption > maximum )
                {
                    this.Usage.State = new OverUsageState(this, usage);
                    this.Usage.State.Subject = usage;
                }
            }
        }
    }
}