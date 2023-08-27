using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using SpotifyAPI.Entities;
using SpotifyAPI.Enums;
using SpotifyAPI.Models;
using SpotifyAPI.Requests;
using SpotifyAPI.Services;
using SpotifyAPI.Variables;

namespace SpotifyAPI.Controllers
{
    [Route(ControllerRoutes.UserController)]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAccessTokenService _accessTokenService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IPasswordResetService _passwordResetService;
        private readonly IRequestValidationService _requestValidationService;
        private readonly IValidator<RegisterUserRequest> _registerUserValidator;
        private readonly IValidator<LoginUserRequest> _loginUserValidator;
        private readonly IValidator<PasswordResetRequest> _passwordResetRequestValidator;
        private readonly IValidator<PasswordResetCompleteRequest> _passwordResetCompleteRequestValidator;

        public UserController(
            IUserService userService,
            IAccessTokenService accessTokenService,
            IRefreshTokenService refreshTokenService,
            IPasswordResetService passwordResetService,
            IRequestValidationService requestValidationService,
            IValidator<RegisterUserRequest> registerUserValidator,
            IValidator<LoginUserRequest> loginUserValidator,
            IValidator<PasswordResetRequest> passwordResetRequestValidator,
            IValidator<PasswordResetCompleteRequest> passwordResetCompleteRequestValidator
            )
        {
            _userService = userService;
            _accessTokenService = accessTokenService;
            _refreshTokenService = refreshTokenService;
            _passwordResetService = passwordResetService;
            _requestValidationService = requestValidationService;
            _registerUserValidator = registerUserValidator;
            _loginUserValidator = loginUserValidator;
            _passwordResetRequestValidator = passwordResetRequestValidator;
            _passwordResetCompleteRequestValidator = passwordResetCompleteRequestValidator;
        }

        [HttpPost]
        public async Task<ActionResult> Register([FromBody] RegisterUserRequest registerUserDto)
        {
            var validationResult = _requestValidationService.ValidateRequest(registerUserDto, _registerUserValidator);
            if (validationResult is BadRequestObjectResult)
            {
                return validationResult;
            }

            bool userAlreadyExist = await _userService.UserExists(registerUserDto.Email, registerUserDto.Nickname);
            if (userAlreadyExist)
            {
                return BadRequest("User with the provided email address or nickname already exists");
            }

            var id = _userService.CreateUser(registerUserDto);
            if (id is null)
            {
                return BadRequest("Something went wrong, please try again");
            }

            return Ok();
        }

        [HttpPost(UserControllerEndpoints.Login)]
        public ActionResult Login([FromBody] LoginUserRequest loginUserDto)
        {
            var validationResult = _requestValidationService.ValidateRequest(loginUserDto, _loginUserValidator);
            if (validationResult is BadRequestObjectResult)
            {
                return validationResult;
            }

            (VerifyUserError error, User user) = _userService.VerifyUser(loginUserDto);

            return error switch
            {
                VerifyUserError.WrongLogin => NotFound("Incorrect login"),
                VerifyUserError.WrongPassword => BadRequest("Incorrect password"),
                _ => SetupUserLogin(user, loginUserDto.RememberMe)
            };
        }

        private ActionResult SetupUserLogin(User user, bool rememberMe)
        {
            SetAccessToken(user);

            if (rememberMe)
            {
                SetRefreshToken(user);
            }

            return Ok();
        }

        private void SetAccessToken(User user)
        {
            string userId = user.Id.ToString();
            string token = _accessTokenService.GenerateAccessToken(userId);
            CookieOptions accessTokenCookieOptions = _accessTokenService.GetAccessTokenCookieOptions();
            Response.Cookies.Append(CookieNames.AccessToken, token, accessTokenCookieOptions);
        }

        private void SetRefreshToken(User user)
        {
            string userId = user.Id.ToString();
            string refreshToken = _refreshTokenService.GenerateRefreshToken(userId);
            _userService.SaveUserRefreshToken(refreshToken, user);
            CookieOptions refreshTokenCookieOptions = _refreshTokenService.GetRefreshTokenCookieOptions();
            Response.Cookies.Append(CookieNames.RefreshToken, refreshToken, refreshTokenCookieOptions);
        }

        [HttpPost(UserControllerEndpoints.PasswordReset)]
        public async Task<IActionResult> PasswordReset([FromBody] PasswordResetRequest passwordResetDto)
        {
            var validationResult = _requestValidationService.ValidateRequest(passwordResetDto, _passwordResetRequestValidator);
            if (validationResult is BadRequestObjectResult)
            {
                return validationResult;
            }

            User? user = _userService.GetUserByLogin(passwordResetDto.Login);

            if (user is null)
            {
                return BadRequest("Account with the provided login doest not exist");
            }

            await _userService.GenerateAndSendPasswordResetToken(user);

            return Ok();
        }

        [HttpPut(UserControllerEndpoints.PasswordResetComplete)]
        public ActionResult PasswordResetComplete([FromBody] PasswordResetCompleteRequest passwordResetCompleteDto, [FromRoute] string token)
        {
            var validationResult = _requestValidationService.ValidateRequest(passwordResetCompleteDto, _passwordResetCompleteRequestValidator);
            if (validationResult is BadRequestObjectResult badRequest)
            {
                return badRequest;
            }

            (string? validationError, string? email) = _passwordResetService.ValidateToken(token);
            if (validationError != null)
            {
                return Unauthorized(validationResult);
            }

            User? user = _userService.CheckUserPasswordResetToken(email, token);
            if (user == null)
            {
                return BadRequest("Invalid token");
            }

            _userService.ChangeUserPassword(user, passwordResetCompleteDto.Password);
            return Ok("Password has changed successfully");
        }
    }
}