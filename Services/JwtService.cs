using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using SpotifyAPI.Middleware;

namespace SpotifyAPI.Services
{
    public interface IJwtService
    {
        SigningCredentials GetSigningCredentials(string secretKey);
        string GenerateToken(List<Claim> claims, string issuer, string audience, string secretKey, DateTime expires);
        ClaimsPrincipal GetPrincipalsFromToken(string token, string secretKey, string tokenErrorMessage);
        CookieOptions CreateCookieOptions(DateTimeOffset tokenLifeTime);
    }

    public class JwtService : IJwtService
    {
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public JwtService(ILogger<RequestLoggingMiddleware> logger)
        {
            _logger = logger;
        }

        public SigningCredentials GetSigningCredentials(string secretKey)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            return new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        }

        public string GenerateToken(List<Claim> claims, string issuer, string audience, string secretKey, DateTime expires)
        {
            var creds = GetSigningCredentials(secretKey);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public ClaimsPrincipal GetPrincipalsFromToken(string token, string secretKey, string tokenErrorMessage)
        {
            try
            {

                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = GetSigningCredentials(secretKey).Key,
                    ValidateIssuer = false,
                    ValidateAudience = false
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

                return principal;
            }
            catch (Exception exception)
            {
                var errorMessage = $"{tokenErrorMessage} Time: {DateTime.Now}. Error message: {exception.Message}";
                _logger.LogError(errorMessage);
            }

            return null;
        }

        public CookieOptions CreateCookieOptions(DateTimeOffset tokenLifeTime)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = tokenLifeTime
            };

            return cookieOptions;
        }
    }
}