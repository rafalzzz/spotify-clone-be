using FluentValidation;
using SpotifyAPI.Extensions;
using SpotifyAPI.Requests;

namespace SpotifyAPI.Validations
{
    public class RegisterUserRequestValidator : AbstractValidator<RegisterUserRequest>
    {
        public RegisterUserRequestValidator()
        {
            RuleFor(requestBody => requestBody.Email)
            .Cascade(CascadeMode.Stop)
            .Email();

            RuleFor(requestBody => requestBody.Password)
            .Cascade(CascadeMode.Stop)
            .Password();

            RuleFor(requestBody => requestBody.Nickname)
            .Cascade(CascadeMode.Stop)
            .Nickname();

            RuleFor(requestBody => requestBody.DateOfBirth)
            .Cascade(CascadeMode.Stop)
            .DateOfBirth();

            RuleFor(requestBody => requestBody.Gender)
            .Cascade(CascadeMode.Stop)
            .Gender();

            RuleFor(requestBody => requestBody.Offers)
            .Cascade(CascadeMode.Stop)
            .IsBoolean();

            RuleFor(requestBody => requestBody.ShareInformation)
            .Cascade(CascadeMode.Stop)
            .IsBoolean();

            RuleFor(requestBody => requestBody.Terms)
            .Cascade(CascadeMode.Stop)
            .IsBoolean();
        }
    }
}