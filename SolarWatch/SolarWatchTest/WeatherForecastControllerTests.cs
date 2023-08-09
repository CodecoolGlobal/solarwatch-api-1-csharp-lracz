using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SolarWatch;
using WeatherApi.Controllers;
using WeatherApi.Services;

namespace SolarWatchTest;

[TestFixture]
public class WeatherForecastControllerTests
{
    [Test]
    public void Get_ReturnsWeatherForecasts()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<WeatherForecastController>>();
        var weatherDataProviderMock = new Mock<IWeatherDataProvider>();
        var jsonProcessorMock = new Mock<IJsonProcessor>();
        var geocodingServiceMock = Mock.Of<IGeocodingService>();
        var sunriseSunsetServiceMock = Mock.Of<ISunriseSunsetService>();

        var controller = new WeatherForecastController(loggerMock.Object, weatherDataProviderMock.Object, jsonProcessorMock.Object,geocodingServiceMock,sunriseSunsetServiceMock);

        // Act
        var result = controller.Get(DateTime.Now);

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
        geocodingServiceMock.Setup(x => x.GetCoordinatesForCityAsync(It.IsAny<string>())).ReturnsAsync((47.497913, 19.040236));

        var controller = new WeatherForecastController(loggerMock.Object, weatherDataProviderMock.Object, jsonProcessorMock.Object,geocodingServiceMock.Object ,sunriseSunsetServiceMock);


        var result = controller.GetSunriseSunset("Budapest", DateTime.Now);

    
        Assert.IsInstanceOf<OkObjectResult>(result.Result);
    }
    
    [Test]
    public async Task GetCurrentReturnsOkResultIfWeatherDataIsValid()
    {
        // Arrange
        var expectedForecast = new WeatherForecast();
        var weatherDataProviderMock = new Mock<IWeatherDataProvider>();
        var geocodingServiceMock = new Mock<IGeocodingService>();
        var loggerMock = new Mock<ILogger<WeatherForecastController>>();
        var jsonProcessorMock = new Mock<IJsonProcessor>();
        var sunriseSunsetServiceMock = Mock.Of<ISunriseSunsetService>();
        geocodingServiceMock.Setup(x => x.GetCoordinatesForCityAsync(It.IsAny<string>())).ReturnsAsync((47.497913, 19.040236));
        var weatherData = "{}";
        weatherDataProviderMock.Setup(x => x.GetCurrentAsync(It.IsAny<double>(), It.IsAny<double>()))
            .ReturnsAsync(weatherData);
        jsonProcessorMock.Setup(x => x.Process(weatherData)).Returns(expectedForecast);
        var controller = new WeatherForecastController(loggerMock.Object, weatherDataProviderMock.Object, jsonProcessorMock.Object,geocodingServiceMock.Object ,sunriseSunsetServiceMock);
        // Act
        var result = await controller.GetCurrent();

        // Assert
        Assert.IsInstanceOf(typeof(OkObjectResult), result.Result);
        Assert.That(((OkObjectResult)result.Result).Value, Is.EqualTo(expectedForecast));
    }
}