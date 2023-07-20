using SpotifyAPI.Enums;

namespace SpotifyAPI.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Nickname { get; set; } = null!;
        public string DateOfBirth { get; set; } = null!;
        public UserGender Gender { get; set; }
        public bool Offers { get; set; }
        public bool ShareInformation { get; set; }
        public bool Terms { get; set; }
        public string RefreshToken { get; set; } = null!;
    }
}