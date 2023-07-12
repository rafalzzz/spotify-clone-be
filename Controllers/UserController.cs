using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using SpotifyAPI.Variables;
using SpotifyAPI.Services;
using SpotifyAPI.Requests;

namespace SpotifyAPI.Controllers
{
    [Route(ControllerRoutes.UserController)]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IRequestValidationService _requestValidationService;
        private readonly IValidator<RegisterUserRequest> _registerUserValidator;

        public UserController(
            IUserService userService,
            IRequestValidationService requestValidationService,
            IValidator<RegisterUserRequest> registerUserValidator
            )
        {
            _userService = userService;
            _requestValidationService = requestValidationService;
            _registerUserValidator = registerUserValidator;
        }

        [HttpPost]
        public ActionResult Register([FromBody] RegisterUserRequest registerUserDto)
        {
            var registerRequestValidation = _registerUserValidator.Validate(registerUserDto);
            var validationResultErrors = _requestValidationService.GetValidationErrorsResult(registerRequestValidation);

            if (validationResultErrors != null)
            {
                return BadRequest(validationResultErrors);
            }

            var id = _userService.CreateUser(registerUserDto);

            if (id is null)
            {
                return BadRequest("User with the provided email address already exists");
            }

            return Ok();
        }
    }
}