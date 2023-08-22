namespace SolarWatch.Model;

public class SunriseSunset
{
    public int Id { get; set; }
    public DateTime Sunrise { get; set; }
    public DateTime Sunset { get; set; }

    // Foreign key relationship
    public int CityId { get; set; }
    public City City { get; set; }
}