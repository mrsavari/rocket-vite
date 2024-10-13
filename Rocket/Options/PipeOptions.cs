namespace Rocket.Options
{
    public class PipeOptions
    {
        public PipeOptions(int cityId, int waterDiameterId)
        {
            CityId = cityId;
            WaterDiameterId = waterDiameterId;
        }

        public int CityId { get; set; }
        public int WaterDiameterId { get; set; }
    }
}