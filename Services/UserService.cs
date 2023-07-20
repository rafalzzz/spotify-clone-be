using SpotifyAPI.Requests;
using SpotifyAPI.Entities;

namespace SpotifyAPI.Services
{
    public interface IUserService
    {
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

        public User GetUserByEmail(string email)
        {
            var user = _dbContext.Users.FirstOrDefault(user => user.Email == email);
            return user;
        }

        public bool CheckIfEmailExist(string email)
        {
            var user = GetUserByEmail(email);
            if (user is null) return false;
            return true;
        }

        public int? CreateUser(RegisterUserRequest registerUserDto)
        {
            if (CheckIfEmailExist(registerUserDto.Email)) return null;

            var passwordHash = _passwordHasher.Hash(registerUserDto.Password);

            Console.Write(registerUserDto.Offers);
            Console.Write(registerUserDto.ShareInformation);
            Console.Write(registerUserDto.Terms);

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