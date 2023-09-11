using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Newtonsoft.Json;
using SolarWatch.Contracts;
using WeatherApi.Services.Authentication;

namespace SolarWatchTest;

public class ApiIntegrationTests
{

    private HttpClient _client;
    private object _authenticationService;

    [SetUp]
    public void Setup()
    {
        _client = new HttpClient();
        _client.BaseAddress = new Uri("http://localhost:5073"); 
    }
    

    [Test]
    public async Task Login_ValidCredentials_ReturnsToken()
    {

        var authRequest = new AuthRequest
        (
             "testuser@example.com",
             "TestPassword123"
        );

        var jsonContent = JsonConvert.SerializeObject(authRequest);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        var requestUri = "/Auth/Login";

        // Act
        var response = await _client.PostAsync(requestUri, content);

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        // Deserialize the response to get the token
        var responseContent = await response.Content.ReadAsStringAsync();
        var authResponse = JsonConvert.DeserializeObject<AuthResponse>(responseContent);

        // Ensure the token is not null or empty
        Assert.IsFalse(string.IsNullOrEmpty(authResponse.Token));
    }

    [Test]
    public async Task GetWeatherForecast_ValidDate_ReturnsWeatherData()
    {

     
        var requestUri = "/WeatherForecast/GetWeatherForecast?date=2023-01-01";


        var response = await _client.GetAsync(requestUri);


        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

    }
    [Test]
    public async Task GetCurrentWeather_ValidCityName_ReturnsWeatherData()
    {

        var cityName = "London";
        var requestUri = $"/WeatherForecast/GetCurrent?cityName={cityName}";
        var loginRequest = new AuthRequest
        (
           "admin@admin.com",
            "admin123"
        );
        var loginResponse = await _client.PostAsJsonAsync("http://localhost:5073/Auth/Login", loginRequest);
 
        if (loginResponse.IsSuccessStatusCode)
        {
      
            var responseContent = await loginResponse.Content.ReadAsStringAsync();
            var authResponse = JsonConvert.DeserializeObject<AuthResponse>(responseContent);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResponse.Token);


            var response = await _client.GetAsync(requestUri);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }
        else
        {
           
            Assert.Fail("Login failed.");
        }
        

    }

    
    [TearDown]
    public void Cleanup()
    {

        _client.Dispose();
    }
}