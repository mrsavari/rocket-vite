using Rocket.Core;
using Rocket.States;
using Rocket.Units;
using Rocket.Options;
using System;

namespace Rocket
{
    public class Bill
    {
        public Bill(TimeSeries series,UsageSubject usage){
            Usage = usage;
            State = new NormalUsageState(Usage);
            State.Subject = Usage;
        }

        public UsageState State { get; set; }
        public UsageSubject Usage { get; private set; }
        public Subscription Subscription { get; set; }
        public SubscriptionSewage SubscriptionSewage { get; set; }
        public SixthRemark SixthRemark { get; set; }
        public PopulationYouth PopulationYouth { get; set; }
        public SecondRemark SecondRemark { get; set; }
        public Seasonal Seasonal { get; set; }
        public FreeWater FreeWater { get; set; }
        public BrokenPipe BrokenPipe { get; set; }
        public WaterCost WaterCost { get; set; }
        public WaterPattern WaterPattern { get; set; }
        public WasteWater WasteWater { get; set; }
        public ThirdRemark ThirdRemark { get; set; }
        public Tax Tax { get; set; }
        public Made7 Made7 { get; set; }

        public long Id { get; set; }
        public long EstateId { get; set; }
        public int CityId { get; set; }
        public int KarBari { get; set; }
        public int SerialNo { get; set; }
        public int tariffUsesId { get; set; }
        public int CategoryUsesNo { get; set; }
        public int Tedad_Vahed { get; set; }
        public int FamillyCount { get; set; }
        public DateTime? Date_Taviz { get; set; }
        public double InitialValueMeter { get; set; }
        public DateTime? ReadDateNow { get; set; }
        public DateTime? ReadDateOld { get; set; }
        public string ReadDate_Now_S { get; set; }
        public int ReadNumOld { get; set; }
        public int ReadNumNow { get; set; }
        public double Consumption { get; set; }
        public double AverageUse { get; set; }
        public double ContractualCapacity { get; set; }
        public int ReadCode { get; set; }
        public int ReadCodeOld { get; set; }
        public bool IsVillage { get; set; }
        public int KindBranchId { get; set; }
        public double SmallAmountThisMonth { get; set; }
        public double SmallAmountLastMonth { get; set; }
        public double CreditAmount { get; set; }
        public double DepreciationCredit { get; set; }
        public bool Approved { get; set; }
        public int Mode { get; set; }
        public bool RBack { get; set; }
        public string Description { get; set; }

        public DateTime? CreatedAt { get; set; }
        public UsageView UsageView { get; set; }
    }
}