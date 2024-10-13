namespace Rocket.Options
{
    public class PipeFAOptions
    {
        public PipeFAOptions(int cityId, int sewageDiameterId, double length)
        {
            CityId = cityId;
            SewageDiameterId = sewageDiameterId;
            Length = length;
        }

        public int CityId { get; set; }
        public int SewageDiameterId { get; set; }
        public double Length { get; set; }
    }
}