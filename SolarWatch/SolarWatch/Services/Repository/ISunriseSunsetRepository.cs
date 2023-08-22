namespace SolarWatch.Model;

public interface ISunriseSunsetRepository
{
    IEnumerable<SunriseSunset> GetAll();
    public Task<SunriseSunset?> GetByCityNameAsync(string city);
    public Task AddAsync(SunriseSunset sunriseSunset);

    public Task DeleteAsync(SunriseSunset sunriseSunset);
    public Task UpdateAsync(SunriseSunset sunriseSunset);
}