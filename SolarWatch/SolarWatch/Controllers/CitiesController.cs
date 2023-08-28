using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SolarWatch.Context;
using SolarWatch.Model;
using WeatherApi.Services;

namespace WeatherApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CitiesController : ControllerBase
{

    private readonly WeatherApiContext _dbContext;


    public CitiesController(WeatherApiContext context)
    {
        _dbContext = context;
    }

    [HttpGet]
    public IActionResult GetCities()
    {
        var cities = _dbContext.Cities.ToList();
        return Ok(cities);
    }
    

    [HttpGet("{id}")]
    public IActionResult GetCity(int id)
    {
        var city = _dbContext.Cities.FirstOrDefault(c => c.Id == id);
        if (city == null)
            return NotFound();

        return Ok(city);
    }

    [HttpPost,Authorize(Roles="Admin")]
    public IActionResult CreateCity([FromBody] City city)
    {
        _dbContext.Cities.Add(city);
        _dbContext.SaveChanges();
        return CreatedAtAction(nameof(GetCity), new { id = city.Id }, city);
    }

    [HttpPut("{id}"),Authorize(Roles="Admin")]
    public IActionResult UpdateCity(int id, [FromBody] City city)
    {
        var existingCity = _dbContext.Cities.FirstOrDefault(c => c.Id == id);
        if (existingCity == null)
            return NotFound();

        existingCity.Name = city.Name;
        existingCity.Latitude = city.Latitude;
        existingCity.Longitude = city.Longitude;

        _dbContext.SaveChanges();
        return NoContent();
    }

    [HttpDelete("{id}"),Authorize(Roles="Admin")]
    public IActionResult DeleteCity(int id)
    {
        var city = _dbContext.Cities.FirstOrDefault(c => c.Id == id);
        if (city == null)
            return NotFound();

        _dbContext.Cities.Remove(city);
        _dbContext.SaveChanges();
        return NoContent();
    }
}