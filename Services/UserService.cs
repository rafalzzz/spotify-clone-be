using SpotifyAPI.Entities;
using SpotifyAPI.Enums;
using SpotifyAPI.Requests;

namespace SpotifyAPI.Services
{
    public interface IUserService
    {
        Task<bool> UserExists(string email, string nickname);
        int? CreateUser(RegisterUserRequest userDto);
        (VerifyUserError error, User user) VerifyUser(LoginUserRequest loginUserDto);
        bool SaveUserRefreshToken(string? token, User user);
        User GetUserByEmail(string email);
        User? GetUserByLogin(string login);
        Task GenerateAndSendPasswordResetToken(User user);
        bool ChangeUserPassword(User user, string password);
    }

    public class UserService : IUserService
    {
        private readonly SpotifyDbContext _dbContext;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IPasswordResetService _passwordResetService;
        public UserService(
            SpotifyDbContext dbContext,
            IPasswordHasher passwordHasher,
            IPasswordResetService passwordResetService
            )
        {
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
            _passwordResetService = passwordResetService;
        }

        public async Task<bool> UserExists(string email, string nickname)
        {
            return await _dbContext.UserExists(email, nickname);
        }

        public int? CreateUser(RegisterUserRequest registerUserDto)
        {
            var passwordHash = _passwordHasher.Hash(registerUserDto.Password);

            User newUser = new User
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
                PasswordResetToken = "",
            };

            _dbContext.Users.Add(newUser);
            _dbContext.SaveChanges();

            return newUser.Id;
        }

        public User GetUserByEmail(string email)
        {
            var user = _dbContext.Users.FirstOrDefault(user => user.Email == email);
            return user;
        }

        private User GetUserByNickname(string nickname)
        {
            var user = _dbContext.Users.FirstOrDefault(user => user.Nickname == nickname);
            return user;
        }

        public User GetUserByLogin(string login)
        {
            bool isEmail = login.Contains("@");

            if (isEmail)
            {
                return GetUserByEmail(login);
            }

            return GetUserByNickname(login);

        }

        private bool VerifyUserPassword(string userPassword, string password)
        {
            return _passwordHasher.Verify(
                    userPassword,
                    password
                );
        }

        public (VerifyUserError error, User user) VerifyUser(LoginUserRequest loginUserDto)
        {
            User? user = GetUserByLogin(loginUserDto.Login);

            if (user is null)
            {
                return (VerifyUserError.WrongLogin, null);
            }

            bool isPasswordCorrect = VerifyUserPassword(
                    user.Password,
                    loginUserDto.Password
                );

            if (!isPasswordCorrect)
            {
                return (VerifyUserError.WrongPassword, null);
            }

            return (VerifyUserError.NoError, user);
        }

        public bool SaveUserRefreshToken(string? token, User user)
        {
            user.RefreshToken = token;
            _dbContext.SaveChanges();

            return true;
        }

        public void SavePasswordResetToken(string? token, User user)
        {
            user.PasswordResetToken = token;
            _dbContext.SaveChanges();
        }

        public async Task GenerateAndSendPasswordResetToken(User user)
        {
            string token = _passwordResetService.GeneratePasswordResetToken(user.Email);
            SavePasswordResetToken(token, user);
            await _passwordResetService.SendPasswordResetToken(user.Email, token);
        }

        public bool ChangeUserPassword(User user, string password)
        {
            var passwordHash = _passwordHasher.Hash(password);
            user.Password = passwordHash;
            user.PasswordResetToken = "";
            _dbContext.SaveChanges();

            return true;
        }
    }
}