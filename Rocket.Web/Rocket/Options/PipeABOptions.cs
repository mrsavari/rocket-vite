namespace Rocket.Options
{
    public class PipeABOptions
    {
        public PipeABOptions(int cityId, int waterDiameterId,double length)
        {
            CityId = cityId;
            WaterDiameterId = waterDiameterId;
            Length = length;
        }

        public int CityId { get; set; }
        public int WaterDiameterId { get; set; }
        public double Length { get; set; }
    }
}