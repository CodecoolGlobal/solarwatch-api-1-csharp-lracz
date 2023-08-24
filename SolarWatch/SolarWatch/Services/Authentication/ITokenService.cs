using Microsoft.AspNetCore.Identity;

namespace WeatherApi.Services.Authentication;

public interface ITokenService
{
    string CreateToken(IdentityUser user, string role);
}