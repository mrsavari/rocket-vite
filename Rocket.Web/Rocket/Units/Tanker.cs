using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.Core;
using Rocket.States;
using Rocket.Options;

namespace Rocket.Units
{
    public class Tanker : Unit, IObserver
    {
        public Tanker(TimeSeries series, TankerOptions options, long? serialNo)
            : base()
        {
            Options = options;
            DataSeries = series;
            SerialNo = serialNo;
        }
        public TankerOptions Options { get; set; }
        public TimeSeries DataSeries { get; set; }
        public long? SerialNo { get; set; }

        protected double GetPrice(bool isVillage, IUsageSubject usage)
        {
            /*if (Options.CityId == 7 && SerialNo != null && SerialNo == 9999)
            {
                return 220000;
            }
            if (Options.CityId == 8 && SerialNo != null && SerialNo == 26123)
            {
                return 336220;
            }*/
            double amount = usage.Consumption;
            InTime timeObj = usage.InTime();
            Frame<Block> WHASH = new Frame<Block>(EntryHash.Empty);
            
            WHASH = DataSeries.Select(new EntryHash(EntryType.Water, Options.CityId,27, 0, 0, Options.ConsumptionType, Options.IsHouse ? 1 : 0)).Transform(new InVolume(amount));

            /*if (isVillage == false)
            {
                //City Mode
                WHASH = DataSeries.Select(new EntryHash(EntryType.Water, Options.CityId, 26, 1, 0, 0, Options.IsVillage ? 1 : 0)).Transform(new InVolume(amount));
            }
            else
            {
                //Village Mode
                WHASH = DataSeries.Select(new EntryHash(EntryType.Water, Options.CityId, 26, 1, 0, Options.IsHouse ? 1 : 0, Options.IsVillage ? 1 : 0)).Transform(new InVolume(amount));
            }*/
            return WHASH.LastOrDefault().Value;

        }

        public override void Update(object sender, EventArgs e)
        {
            Reset(TransactionSource.Tanker);
            IUsageSubject usage = Subject as IUsageSubject;
            InTime timeObj = usage.InTime();
            double amount = usage.Consumption;
            int cityId = Options.CityId;
            if (Options.CycleId != null)
            {
                if (new int[] { 2620, 5947 }.Contains(Options.CycleId.Value))
                {
                    cityId = 5;
                }
                if (new int[] { 1632 }.Contains(Options.CycleId.Value))
                {
                    cityId = 24;
                }
                if (new int[] { 2582, 2645 }.Contains(Options.CycleId.Value))
                {
                    cityId = 76;
                }
                if (new int[] { 6368 }.Contains(Options.CycleId.Value))
                {
                    cityId = 49;
                }
            }

            //LinkedListNode<Dictionary<string, Block>> cityBlock = DataSeries.Select(new EntryHash(EntryType.Water, cityId, 0, 14, 0, 0, 1)).Transform(new InVolume(amount)).Extract<LinkedList<Dictionary<string, Block>>>(new Link(timeObj.From, timeObj.To, usage)).First;
            //double cityRate = (cityBlock == null || cityBlock.Value["14"].Value < 1) ? 1 : cityBlock.Value["14"].Value;

            Frame<Block> WaterBlock = new Frame<Block>(EntryHash.Empty);
            int isNullCategory = Options.CategoryId == null ? 0 : Options.CategoryId.Value;
            if(Options.CategoryId == null)
                WaterBlock = DataSeries.Select(new EntryHash(EntryType.Water, Options.CityId, 27, 0, 0, Options.ConsumptionType, Options.IsHouse ? 1 : 0)).Transform(new InVolume(amount));
            else
                WaterBlock = DataSeries.Select(new EntryHash(EntryType.Water, 0, 27, 0, Options.CategoryId.Value, 0, 0)).Transform(new InVolume(amount));
            double waterRate = 0.0;
            double defaultRate = WaterBlock.LastOrDefault().Value;
            waterRate = defaultRate + 0;
            if (Options.WaterRate > 0)
            {
                waterRate = Options.WaterRate;
            }
            ApplyCityRate applyCityRate = new ApplyCityRate(DataSeries);
            Water = applyCityRate.Apply(usage, cityId, Options.Tariff, Options.IsVillage, 1, (r, d) =>
            {
                return amount * waterRate * 1;
            });
           // Water = amount * waterRate * cityRate;
            if (Options.Excludes.Contains(TankerSource.Transportation) == false)
            {
                //LinkedListNode<Dictionary<string, Block>> distanceBlock = DataSeries.Select(new EntryHash(EntryType.Water, Options.CityId, 26, 2, 0, 0, Options.IsVillage ? 1 : 0)).Transform(new InVolume(amount)).Extract<LinkedList<Dictionary<string, Block>>>(new Link(timeObj.From, timeObj.To, usage)).First;
                //double distanceRate = (distanceBlock == null || distanceBlock.Value["2"].Value < 1) ? 1 : distanceBlock.Value["2"].Value;
                //Distence = Options.Distence * distanceRate * (amount / Options.Capacity);
                Transport = amount * Options.Distence * 149000; 
            }
            if (Options.Excludes.Contains(TankerSource.Seasonal) == false)
            {
                LinkedListNode<Dictionary<string, Block>> seasonalBlock = DataSeries.Select(new EntryHash(EntryType.Water, Options.CityId, Options.Tariff == 11 ? 11 : 0, 20, 0, 0, 1)).Transform(new InVolume(amount)).Extract<LinkedList<Dictionary<string, Block>>>(new Link(timeObj.From, timeObj.To, usage)).Last;
                double seasonalRate = (seasonalBlock == null) ? 0 : seasonalBlock.Value["20"].Value;
                Seasonal = Water * seasonalRate;
            }

            LinkedListNode<Dictionary<string, Block>> taxBlock = DataSeries.Select(new EntryHash(EntryType.Water, Options.CityId, 0, 19, 0, 0, 1)).Extract<LinkedList<Dictionary<string, Block>>>(new Link(timeObj.From, timeObj.To, usage)).First;
            double taxRate = (taxBlock == null) ? 0.1 : taxBlock.Value["19"].Value;
            Tax = (Water + Seasonal + Transport + Distence) * taxRate;

            /*if (Options.Excludes.Contains(TankerSource.ThirdRemark) == false && Options.ConsumptionType == 1)
            {
                LinkedListNode<Dictionary<string, Block>> thirdFrame = DataSeries.Select(new EntryHash(EntryType.Water, Options.CityId, Options.Karbari, 23, 0, 0, 1)).Transform(new InVolume(amount)).Extract<LinkedList<Dictionary<string, Block>>>(new Link(DateTime.Now, DateTime.Now, usage)).First;
                Block thirdBlock = (thirdFrame == null) ? new Block(DateTime.Now, 0, 0, 1, "23", 0) : thirdFrame.Value["23"];
                if (Options.IsVillage == false)
                {
                    ThirdRemark += thirdBlock.Extra;
                    ThirdRemark += ((amount) * thirdBlock.Value);
                }
            }
            if (Options.CityId == 29 && SerialNo != null && SerialNo == 2400)
            {
                Water = amount * 300000;
                return;
            }
            Frame<Block> WHASH = new Frame<Block>(EntryHash.Empty);
            double defaultCost = GetPrice(false, usage);
            
            Frame<Block> DHASH = new Frame<Block>(EntryHash.Empty);
            DHASH = DataSeries.Select(new EntryHash(EntryType.Water, Options.CityId, 26, 2, 0, 0, Options.IsVillage ? 1 : 0)).Transform(new InVolume(amount));
            if (Options.RO)
            {
                WHASH = DataSeries.Select(new EntryHash(EntryType.Water, Options.CityId, 26, 3, 0, 0, 0)).Transform(new InVolume(amount));
                LinkedListNode<Dictionary<string, Block>> RoRate = WHASH.Extract<LinkedList<Dictionary<string, Block>>>(new Link(timeObj.From, timeObj.To, usage)).Last;
                if (RoRate != null)
                {
                    waterPrice = RoRate.Value["3"].Value;
                }
            }
            Water = amount * waterPrice;
            
            Frame<Block> CHash = DataSeries.Select(new EntryHash(EntryType.Water, Options.CityId, 0, 14, 0, 0, 1)).Transform(new InVolume(amount));
            LinkedList<Dictionary<string, Block>>  rates = CHash.Extract<LinkedList<Dictionary<string, Block>>>(new Link(timeObj.From, timeObj.To, usage));
            LinkedListNode<Dictionary<string, Block>> currentStep = rates.First;
            if (currentStep != null)
            {
                double cityRate = (currentStep.Value["14"].Value < 1) ? 1 : currentStep.Value["14"].Value;
                Water = Water * cityRate;
                Tax = Water;
            }
            if (Options.Transportation)
            {
                //if (amount < Options.Capacity)
                //    amount = Options.Capacity;
                Transport = amount * defaultCost * 2;//amount * waterPrice * 2;
                rates = DHASH.Extract<LinkedList<Dictionary<string, Block>>>(new Link(timeObj.From, timeObj.To, usage));
                currentStep = rates.First;
                if (currentStep != null )
                {
                    Distence = Options.Distence * distanceRate * (amount / Options.Capacity);
                }
            }
            bool type = (Options.Tariff == 11); 
            Frame<Block> SHash = DataSeries.Select(new EntryHash(EntryType.Water, Options.CityId, Options.Tariff == 11 ? 11 : 0, 20, 0, 0, 1)).Transform(new InVolume(amount));
            rates = SHash.Extract<LinkedList<Dictionary<string, Block>>>(new Link(timeObj.To, timeObj.To, usage));
            currentStep = rates.First;
            Seasonal = Water * currentStep.Value["20"].Value;

            Frame<Block> SRHash = DataSeries.Select(new EntryHash(EntryType.Water, Options.CityId, 0, 19, 0, 0, 1)); 
            
            rates = SRHash.Extract<LinkedList<Dictionary<string, Block>>>(new Link(timeObj.From, timeObj.To, usage));
            currentStep = rates.First;
            if (currentStep != null)
            {
                Tax = (Water + Seasonal + Transport + Distence) * currentStep.Value["19"].Value;
            }

            if (Options.IsVillage == false)
            {
                rates = DataSeries.Select(new EntryHash(EntryType.Water, Options.CityId, Options.Karbari, 23, 0, 0, 1)).Transform(new InVolume(amount)).Extract<LinkedList<Dictionary<string, Block>>>(new Link(DateTime.Now, DateTime.Now, usage));
                ThirdRemark += (rates.First.Value["23"].Extra);
                ThirdRemark += ((amount) * rates.First.Value["23"].Value);
            }*/

            Price = Water + Seasonal + Transport + Distence + ThirdRemark + Tax;
        }

        public double Price { get; set; }

        public double Water { get; set; }

        public double Transport { get; set; }

        public double Distence { get; set; }

        public double ThirdRemark { get; set; }

        public double Seasonal { get; set; }

        public double Tax { get; set; }
    }
}