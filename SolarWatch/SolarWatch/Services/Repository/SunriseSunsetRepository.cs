using Microsoft.EntityFrameworkCore;
using SolarWatch.Context;

namespace SolarWatch.Model;

public class SunriseSunsetRepository: ISunriseSunsetRepository
{
    private readonly WeatherApiContext _dbContext;

    public SunriseSunsetRepository(WeatherApiContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IEnumerable<SunriseSunset> GetAll()
    {
        return _dbContext.SunriseSunsets.ToList();
    }

    public async Task<SunriseSunset?> GetByCityNameAsync(string cityName)
    {
        return await _dbContext.SunriseSunsets
            .Include(ss => ss.City)
            .FirstOrDefaultAsync(ss => ss.City.Name == cityName);
    }

    public async Task AddAsync(SunriseSunset sunriseSunset)
    {
        _dbContext.SunriseSunsets.Add(sunriseSunset);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(SunriseSunset sunriseSunset)
    {
        _dbContext.SunriseSunsets.Remove(sunriseSunset);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(SunriseSunset sunriseSunset)
    {
        _dbContext.SunriseSunsets.Update(sunriseSunset);
        await _dbContext.SaveChangesAsync();
    }
}
