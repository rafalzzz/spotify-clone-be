using SpotifyAPI.Enums;

namespace SpotifyAPI.Requests
{
    public class LoginUserRequest
    {
        public string Login { get; set; } = null!;
        public string Password { get; set; } = null!;
        public bool RememberMe { get; set; }

    }
}