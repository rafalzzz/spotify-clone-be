using SpotifyAPI.Enums;

namespace SpotifyAPI.Requests
{
    public class RegisterUserRequest
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Nickname { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public UserGender Gender { get; set; }
        public string Prefix { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;

    }
}