using SpotifyAPI.Requests;
using SpotifyAPI.Entities;

namespace SpotifyAPI.Services
{
    public interface IUserService
    {
        Task<bool> UserExists(string email, string nickname);
        int? CreateUser(RegisterUserRequest userDto);
    }

    public class UserService : IUserService
    {
        private readonly SpotifyDbContext _dbContext;
        private readonly IPasswordHasher _passwordHasher;
        public UserService(
            SpotifyDbContext dbContext,
            IPasswordHasher passwordHasher
            )
        {
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
        }

        public async Task<bool> UserExists(string email, string nickname)
        {
            return await _dbContext.UserExists(email, nickname);
        }

        public int? CreateUser(RegisterUserRequest registerUserDto)
        {
            var passwordHash = _passwordHasher.Hash(registerUserDto.Password);

            var newUser = new User
            {
                Email = registerUserDto.Email,
                Password = passwordHash,
                Nickname = registerUserDto.Nickname,
                DateOfBirth = registerUserDto.DateOfBirth,
                Gender = registerUserDto.Gender,
                Offers = registerUserDto.Offers,
                ShareInformation = registerUserDto.ShareInformation,
                Terms = registerUserDto.Terms,
                RefreshToken = "",
            };

            _dbContext.Users.Add(newUser);
            _dbContext.SaveChanges();

            return newUser.Id;
        }
    }
}