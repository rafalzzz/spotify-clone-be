using System.IdentityModel.Tokens.Jwt;
using System.Security;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SpotifyAPI.Enums;
using SpotifyAPI.Extensions;
using SpotifyAPI.Middleware;
using SpotifyAPI.Models;
using SpotifyAPI.Variables;

namespace SpotifyAPI.Services
{
    public interface IPasswordResetService
    {
        string GeneratePasswordResetToken(string userEmail);
        Task SendPasswordResetToken(string email, string token);
        ActionResult ValidateToken(string token);
        string GetEmailFromToken(object tokenResult);
    }

    public class PasswordResetService : IPasswordResetService
    {
        private readonly IJwtService _jwtService;
        private readonly PasswordResetSettings _passwordResetSettings;
        private readonly IEmailService _emailService;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public PasswordResetService(
            IJwtService jwtService,
            IOptions<PasswordResetSettings> passwordResetSettings,
            IEmailService emailService,
            ILogger<RequestLoggingMiddleware> logger
            )
        {
            _jwtService = jwtService;
            _passwordResetSettings = passwordResetSettings.Value;
            _emailService = emailService;
            _logger = logger;
        }

        private List<Claim> GetPasswordResetTokenClaims(string userEmail)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, userEmail),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            return claims;
        }

        public string GeneratePasswordResetToken(string userEmail)
        {
            List<Claim> claims = GetPasswordResetTokenClaims(userEmail);
            string passwordResetSecretKey = Environment.GetEnvironmentVariable(EnvironmentVariables.PasswordResetSecretKey);
            DateTime expires = DateTime.Now.AddMinutes(_passwordResetSettings.TokenLifeTime);
            Console.WriteLine(_passwordResetSettings.TokenLifeTime);
            Console.WriteLine(expires);

            return _jwtService.GenerateToken(claims, _passwordResetSettings.Issuer, _passwordResetSettings.Audience, passwordResetSecretKey, expires);
        }

        public async Task SendPasswordResetToken(string email, string token)
        {
            string emailTitle = "Password reset";

            string clientUrl = Environment.GetEnvironmentVariable(EnvironmentVariables.ClientUrl);
            string passwordResetUrl = $"{clientUrl}/{token}";
            string emailContent = $@"
                <html>
                    <body style='width: 100%;'>
                        <h3 style='text-align: center;'>To reset your password</h3>
                        <div style='text-align: center; margin-top: 10px;'>
                            <a href='{passwordResetUrl}' style='background-color: #4CAF50; color: white; padding: 14px 20px; text-align: center; text-decoration: none; display: inline-block; border: none; cursor: pointer;'>Open this link</a>
                        </div>
                    </body>
                </html>";


            await _emailService.SendEmailAsync(email, emailTitle, emailContent);
        }

        private string GetPasswordResetSecretKey()
        {
            return Environment.GetEnvironmentVariable(EnvironmentVariables.PasswordResetSecretKey);
        }

        private SecurityKey GetSigningCredentialsKey(string secretKey)
        {
            return _jwtService.GetSigningCredentials(secretKey).Key;
        }

        private TokenValidationParameters CreateTokenValidationParameters(SecurityKey key)
        {
            return new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = true,
                ValidIssuer = _passwordResetSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = _passwordResetSettings.Audience,
                ValidateLifetime = true
            };
        }

        private JwtSecurityToken ValidateJwtToken(string token, TokenValidationParameters tokenValidationParameters)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            JwtSecurityToken jwtSecurityToken = securityToken as JwtSecurityToken;

            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityException("Incorrect token");
            }

            return jwtSecurityToken;
        }

        private Claim GetEmailClaim(JwtSecurityToken jwtSecurityToken)
        {
            return jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
        }

        private object LogErrorAndReturnTokenStatus(string errorMessage, VerifyPasswordResetToken status)
        {
            _logger.LogError(errorMessage);
            return status;
        }

        private object GetEmailFromPasswordResetToken(string token)
        {
            try
            {
                string passwordResetSecretKey = GetPasswordResetSecretKey();
                SecurityKey key = GetSigningCredentialsKey(passwordResetSecretKey);

                TokenValidationParameters tokenValidationParameters = CreateTokenValidationParameters(key);
                JwtSecurityToken jwtSecurityToken = ValidateJwtToken(token, tokenValidationParameters);

                Claim emailClaim = GetEmailClaim(jwtSecurityToken);
                return emailClaim?.Value;
            }
            catch (SecurityTokenExpiredException)
            {
                string errorMessage = $"Token has expired. Time: {DateTime.Now}.";
                return LogErrorAndReturnTokenStatus(errorMessage, VerifyPasswordResetToken.TokenHasExpired);
            }
            catch (SecurityException exception)
            {
                string errorMessage = $"Reset password token validation error. Time: {DateTime.Now}. Error message: {exception.Message}";
                return LogErrorAndReturnTokenStatus(errorMessage, VerifyPasswordResetToken.TokenValidationError);
            }
            catch (Exception exception)
            {
                string errorMessage = $"Unexpected error during token validation. Time: {DateTime.Now}. Error message: {exception.Message}";
                return LogErrorAndReturnTokenStatus(errorMessage, VerifyPasswordResetToken.TokenValidationError);
            }
        }

        public ActionResult ValidateToken(string token)
        {
            object emailFromTokenResult = GetEmailFromPasswordResetToken(token);
            if (emailFromTokenResult is VerifyPasswordResetToken tokenStatus)
            {
                return tokenStatus switch
                {
                    VerifyPasswordResetToken.TokenHasExpired => new UnauthorizedObjectResult("Token has expired"),
                    VerifyPasswordResetToken.TokenValidationError => new UnauthorizedObjectResult("Invalid token"),
                    _ => new UnauthorizedObjectResult("Unknown token error")
                };
            }
            return null;
        }

        public string GetEmailFromToken(object tokenResult)
        {
            return tokenResult.ToString();
        }
    }
}