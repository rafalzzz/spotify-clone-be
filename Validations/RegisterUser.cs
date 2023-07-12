using FluentValidation;
using SpotifyAPI.Extensions;
using SpotifyAPI.Requests;

namespace SpotifyAPI.Validations
{
    public class RegisterUserRequestValidator : AbstractValidator<RegisterUserRequest>
    {
        public RegisterUserRequestValidator()
        {
            RuleFor(requestBody => requestBody.FirstName)
            .Cascade(CascadeMode.Stop)
            .FirstName();

            RuleFor(requestBody => requestBody.LastName)
            .Cascade(CascadeMode.Stop)
            .LastName();

            RuleFor(requestBody => requestBody.Nickname)
            .Cascade(CascadeMode.Stop)
            .Nickname();

            RuleFor(requestBody => requestBody.Email)
            .Cascade(CascadeMode.Stop)
            .Email();

            RuleFor(requestBody => requestBody.Password)
            .Cascade(CascadeMode.Stop)
            .Password();

            RuleFor(requestBody => requestBody.Gender)
            .Cascade(CascadeMode.Stop)
            .Gender();

            RuleFor(requestBody => requestBody.Prefix)
           .Cascade(CascadeMode.Stop)
           .Prefix();

            RuleFor(requestBody => requestBody.PhoneNumber)
           .Cascade(CascadeMode.Stop)
           .PhoneNumber();
        }
    }
}