using Microsoft.EntityFrameworkCore;
using SolarWatch.Context;

namespace SolarWatch.Model;

public class CityRepository : ICityRepository
{
    private readonly WeatherApiContext _dbContext;

    public CityRepository(WeatherApiContext dbContext)
    {
        _dbContext = dbContext;
    }
    public IEnumerable<City> GetAll()
    {
        return _dbContext.Cities.ToList();
    }

    public async Task<City?> GetByNameAsync(string name)
    {
        return await _dbContext.Cities.FirstOrDefaultAsync(c => c.Name == name);
    }

    public async Task AddAsync(City city)
    {
        _dbContext.Add(city);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(City city)
    {
        _dbContext.Remove(city);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(City city)
    {  
        _dbContext.Update(city);
        await _dbContext.SaveChangesAsync();
    }
}