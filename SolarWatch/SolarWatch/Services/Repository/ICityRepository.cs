namespace SolarWatch.Model;

public interface ICityRepository
{
    IEnumerable<City> GetAll();
    public Task<City?> GetByNameAsync(string name);
    public Task AddAsync(City city);

    public Task DeleteAsync(City city);
    public Task UpdateAsync(City city);
}