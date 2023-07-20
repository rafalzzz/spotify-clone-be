using SpotifyAPI.Enums;

namespace SpotifyAPI.Requests
{
    public class RegisterUserRequest
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Nickname { get; set; } = null!;
        public string DateOfBirth { get; set; } = null!;
        public UserGender Gender { get; set; }
        public bool Offers { get; set; }
        public bool ShareInformation { get; set; }
        public bool Terms { get; set; }

    }
}