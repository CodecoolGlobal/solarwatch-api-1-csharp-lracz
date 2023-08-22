using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SolarWatch;
using SolarWatch.Model;
using WeatherApi.Controllers;
using WeatherApi.Services;

namespace SolarWatchTest;

[TestFixture]
public class WeatherForecastControllerTests
{
    private Mock<ILogger<WeatherForecastController>> _loggerMock;
    private Mock<IWeatherDataProvider> _weatherDataProviderMock;
    private Mock<ICityRepository> _cityRepositoryMock;
    private WeatherForecastController _controller;
    private Mock<IJsonProcessor> _jsonProcessorMock;
    private IGeocodingService _geocodingServiceMock;
    private ISunriseSunsetService _sunriseSunsetServiceMock;
    private ISunriseSunsetRepository _sunriseSunsetRepositoryMock;

    [SetUp]
    public void SetUp()
    {
        _loggerMock = new Mock<ILogger<WeatherForecastController>>();
        _weatherDataProviderMock = new Mock<IWeatherDataProvider>();
        _jsonProcessorMock = new Mock<IJsonProcessor>();
        _cityRepositoryMock = new Mock<ICityRepository>();
        _geocodingServiceMock = Mock.Of<IGeocodingService>();
         _sunriseSunsetServiceMock = Mock.Of<ISunriseSunsetService>();
         _sunriseSunsetServiceMock = Mock.Of<ISunriseSunsetService>();
        _sunriseSunsetRepositoryMock = Mock.Of<ISunriseSunsetRepository>();
        _controller = new WeatherForecastController(_loggerMock.Object, _weatherDataProviderMock.Object,
            _jsonProcessorMock.Object,_geocodingServiceMock,_sunriseSunsetServiceMock ,_cityRepositoryMock.Object,_sunriseSunsetRepositoryMock );
    }
    [Test]
    public void Get_ReturnsWeatherForecasts()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<WeatherForecastController>>();
        var weatherDataProviderMock = new Mock<IWeatherDataProvider>();
        var jsonProcessorMock = new Mock<IJsonProcessor>();
        var geocodingServiceMock = Mock.Of<IGeocodingService>();
        var sunriseSunsetServiceMock = Mock.Of<ISunriseSunsetService>();
        var cityRepositoryMock = new Mock<ICityRepository>();

        //var controller = new WeatherForecastController(loggerMock.Object, weatherDataProviderMock.Object, jsonProcessorMock.Object,geocodingServiceMock,sunriseSunsetServiceMock, cityRepositoryMock.Object);

        // Act
        var result = _controller.Get(DateTime.Now);

        // Assert
        Assert.IsInstanceOf<OkObjectResult>(result.Result);
        var okResult = result.Result as OkObjectResult;
        Assert.IsInstanceOf<IEnumerable<WeatherForecast>>(okResult.Value);
    }

    [Test]
    public void GetSunriseSunset_ReturnsOkResult()
    {
    
        var loggerMock = new Mock<ILogger<WeatherForecastController>>();
        var geocodingServiceMock = new Mock<IGeocodingService>();
        var weatherDataProviderMock = new Mock<IWeatherDataProvider>(); // Correct interface
        var jsonProcessorMock = new Mock<IJsonProcessor>();
        var sunriseSunsetServiceMock = Mock.Of<ISunriseSunsetService>();
        var cityRepositoryMock = new Mock<ICityRepository>();

        geocodingServiceMock.Setup(x => x.GetCoordinatesForCityAsync(It.IsAny<string>())).ReturnsAsync((47.497913, 19.040236));

        //var controller = new WeatherForecastController(loggerMock.Object, weatherDataProviderMock.Object, jsonProcessorMock.Object,geocodingServiceMock.Object ,sunriseSunsetServiceMock,cityRepositoryMock.Object);


        var result = _controller.GetSunriseSunset("Budapest", DateTime.Now);

    
        Assert.IsInstanceOf<OkObjectResult>(result.Result);
    }
    
    [Test]
    public async Task 
        GetCurrentReturnsOkResultIfWeatherDataIsValid()
    {
        // Arrange
        var expectedForecast = new WeatherForecast();
    
        var weatherData = "{}";
        _weatherDataProviderMock.Setup(x => x.GetCurrentAsync(It.IsAny<double>(), It.IsAny<double>()))
            .ReturnsAsync(weatherData);
        _jsonProcessorMock.Setup(x => x.Process(weatherData)).Returns(expectedForecast);
        _cityRepositoryMock.Setup(x => x.GetByNameAsync("Budapest")).ReturnsAsync(new City { Name = "Budapest" });

        // Act
        var result = await _controller.GetCurrent("Budapest");

        var okObjectResult = (OkObjectResult)result.Result;
        var actualValue = okObjectResult.Value as dynamic;
        // Assert
        //  return Ok(new { WeatherData = processedWeatherData, SunriseSunset = sunriseSunset });
        
        
        
        Assert.IsInstanceOf(typeof(OkObjectResult), result.Result);
        Assert.That(actualValue.WeatherData, Is.EqualTo(expectedForecast));
  

      
    }
}

