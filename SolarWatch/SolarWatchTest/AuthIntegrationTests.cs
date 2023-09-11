using System.Net;
using System.Text;
using System.Text.Json;
using SolarWatch.Contracts;

namespace SolarWatchTest;

[TestFixture]
public class AuthIntegrationTests
{
    private HttpClient _client;

    [SetUp]
    public void Setup()
    {
        _client = new HttpClient();
        _client.BaseAddress = new Uri("http://localhost:5073"); 
    }

    [Test]
    public async Task RegisterUser_ValidRegistrationRequest_ReturnsCreatedResponse()
    {

        var registrationRequest = new RegistrationRequest
        (
          "testuser@example.com",
          "testuser",
          "TestPassword123"
        );

        var content = new StringContent(JsonSerializer.Serialize(registrationRequest), Encoding.UTF8, "application/json");
        var requestUri = "/Auth/Register";

        var response = await _client.PostAsync(requestUri, content);

        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

    }

    [TearDown]
    public void Cleanup()
    {
        _client.Dispose();
    }
}