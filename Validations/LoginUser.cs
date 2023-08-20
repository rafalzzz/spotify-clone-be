using FluentValidation;
using SpotifyAPI.Extensions;
using SpotifyAPI.Requests;

namespace SpotifyAPI.Validations
{
    public class LoginUserRequestValidator : AbstractValidator<LoginUserRequest>
    {
        public LoginUserRequestValidator()
        {
            RuleFor(requestBody => requestBody.Login)
            .Cascade(CascadeMode.Stop)
            .Login();

            RuleFor(requestBody => requestBody.Password)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Password is required");

            RuleFor(requestBody => requestBody.RememberMe)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithMessage("RememberMe is required");
        }
    }
}