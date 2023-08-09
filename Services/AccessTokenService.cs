using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using SpotifyAPI.Models;
using SpotifyAPI.Variables;

namespace SpotifyAPI.Services
{
    public interface IAccessTokenService
    {
        string GenerateAccessToken(string userId);
        ClaimsPrincipal GetPrincipalsFromAccessToken(string token);
        CookieOptions GetAccessTokenCookieOptions();

    }

    public class AccessTokenService : IAccessTokenService
    {
        private readonly string _errorMessage = "Access token validation error";
        private readonly JwtSettings _jwtSettings;
        private readonly IJwtService _jwtService;

        public AccessTokenService(
            IOptions<JwtSettings> jwtSettings,
            IJwtService jwtService
            )
        {
            _jwtSettings = jwtSettings.Value;
            _jwtService = jwtService;
        }

        private List<Claim> GetAccessTokenClaims(string userId)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            return claims;
        }

        public string GenerateAccessToken(string userId)
        {
            List<Claim> claims = GetAccessTokenClaims(userId);
            string secretKey = Environment.GetEnvironmentVariable(EnvironmentVariables.SecretKey);
            DateTime expires = DateTime.Now.AddMinutes(_jwtSettings.TokenLifeTime);

            return _jwtService.GenerateToken(claims, _jwtSettings.Issuer, _jwtSettings.Audience, secretKey, expires);
        }

        public ClaimsPrincipal GetPrincipalsFromAccessToken(string token)
        {
            var secretKey = Environment.GetEnvironmentVariable(EnvironmentVariables.SecretKey);
            return _jwtService.GetPrincipalsFromToken(token, secretKey, _errorMessage);
        }

        public CookieOptions GetAccessTokenCookieOptions()
        {
            DateTimeOffset expires = DateTimeOffset.UtcNow.AddMinutes(_jwtSettings.TokenLifeTime);
            return _jwtService.CreateCookieOptions(expires);
        }
    }
}