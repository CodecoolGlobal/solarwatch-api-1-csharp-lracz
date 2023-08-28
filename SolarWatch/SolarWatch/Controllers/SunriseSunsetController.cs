using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SolarWatch.Context;
using SolarWatch.Model;

namespace WeatherApi.Controllers;


[ApiController]
[Route("api/[controller]")]
public class SunriseSunsetsController : ControllerBase
{
    private readonly WeatherApiContext _dbContext;


    public SunriseSunsetsController(WeatherApiContext context)
    {
        _dbContext = context;
    }

    [HttpGet]
    public IActionResult GetSunriseSunsets()
    {
        var sunriseSunsets = _dbContext.SunriseSunsets.Include(ss => ss.City).ToList();
        return Ok(sunriseSunsets);
    }

    [HttpGet("{id}")]
    public IActionResult GetSunriseSunset(int id)
    {
        var sunriseSunset = _dbContext.SunriseSunsets.Include(ss => ss.City).FirstOrDefault(ss => ss.Id == id);
        if (sunriseSunset == null)
            return NotFound();

        return Ok(sunriseSunset);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public IActionResult CreateSunriseSunset([FromBody] SunriseSunset sunriseSunset)
    {
        _dbContext.SunriseSunsets.Add(sunriseSunset);
        _dbContext.SaveChanges();
        return CreatedAtAction(nameof(GetSunriseSunset), new { id = sunriseSunset.Id }, sunriseSunset);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public IActionResult UpdateSunriseSunset(int id, [FromBody] SunriseSunset sunriseSunset)
    {
        var existingSunriseSunset = _dbContext.SunriseSunsets.FirstOrDefault(ss => ss.Id == id);
        if (existingSunriseSunset == null)
            return NotFound();

        existingSunriseSunset.Sunrise = sunriseSunset.Sunrise;
        existingSunriseSunset.Sunset = sunriseSunset.Sunset;

        _dbContext.SaveChanges();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public IActionResult DeleteSunriseSunset(int id)
    {
        var sunriseSunset = _dbContext.SunriseSunsets.FirstOrDefault(ss => ss.Id == id);
        if (sunriseSunset == null)
            return NotFound();

        _dbContext.SunriseSunsets.Remove(sunriseSunset);
        _dbContext.SaveChanges();
        return NoContent();
    }
}