using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using SpotifyAPI.Entities;
using SpotifyAPI.Enums;
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

        public UserController(
            IUserService userService,
            IAccessTokenService accessTokenService,
            IRefreshTokenService refreshTokenService,
            IPasswordResetService passwordResetService,
            IRequestValidationService requestValidationService,
            IValidator<RegisterUserRequest> registerUserValidator,
            IValidator<LoginUserRequest> loginUserValidator,
            IValidator<PasswordResetRequest> passwordResetRequestValidator
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
        }

        [HttpPost]
        public async Task<ActionResult> Register([FromBody] RegisterUserRequest registerUserDto)
        {
            var registerRequestValidation = _registerUserValidator.Validate(registerUserDto);
            var validationResultErrors = _requestValidationService.GetValidationErrorsResult(registerRequestValidation);

            if (validationResultErrors != null)
            {
                return BadRequest(validationResultErrors);
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
            var loginRequestValidation = _loginUserValidator.Validate(loginUserDto);
            var validationResultErrors = _requestValidationService.GetValidationErrorsResult(loginRequestValidation);

            if (validationResultErrors != null)
            {
                return BadRequest(validationResultErrors);
            }

            (VerifyUserError error, User user) verifiedUser = _userService.VerifyUser(loginUserDto);

            switch (verifiedUser.error)
            {
                case VerifyUserError.WrongLogin:
                    return NotFound("Incorrect login");
                case VerifyUserError.WrongPassword:
                    return BadRequest("Incorrect password");
                default:
                    string userId = verifiedUser.user.Id.ToString();
                    string token = _accessTokenService.GenerateAccessToken(userId);
                    CookieOptions accessTokenCookieOptions = _accessTokenService.GetAccessTokenCookieOptions();
                    Response.Cookies.Append(CookieNames.AccessToken, token, accessTokenCookieOptions);

                    if (loginUserDto.RememberMe)
                    {
                        string refreshToken = _refreshTokenService.GenerateRefreshToken(userId);
                        _userService.SaveUserRefreshToken(refreshToken, verifiedUser.user);
                        CookieOptions refreshTokenCookieOptions = _refreshTokenService.GetRefreshTokenCookieOptions();
                        Response.Cookies.Append(CookieNames.RefreshToken, refreshToken, refreshTokenCookieOptions);
                    }


                    return Ok();
            }
        }

        [HttpPost(UserControllerEndpoints.PasswordReset)]
        public async Task<IActionResult> PasswordReset([FromBody] PasswordResetRequest passwordResetDto)
        {
            var passwordResetRequestValidation = _passwordResetRequestValidator.Validate(passwordResetDto);
            var validationResultErrors = _requestValidationService.GetValidationErrorsResult(passwordResetRequestValidation);

            if (validationResultErrors != null)
            {
                return BadRequest(validationResultErrors);
            }

            var user = _userService.GetUserByLogin(passwordResetDto.Login);

            if (user is null)
            {
                return BadRequest("Account with the provided login doest not exist");
            }

            await _passwordResetService.SendPasswordResetToken(user.Email);

            return Ok();
        }
    }
}