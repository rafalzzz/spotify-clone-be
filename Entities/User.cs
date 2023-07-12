using SpotifyAPI.Enums;

namespace SpotifyAPI.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Nickname { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public UserGender Gender { get; set; }
        public string Prefix { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
    }
}