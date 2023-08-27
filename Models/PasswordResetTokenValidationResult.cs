using SpotifyAPI.Enums;

namespace SpotifyAPI.Models
{
    public class PasswordResetTokenValidationResult
    {
        public string? Email { get; set; }
        public VerifyPasswordResetToken? ErrorStatus { get; set; }
    }
}