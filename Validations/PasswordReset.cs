using FluentValidation;
using SpotifyAPI.Extensions;
using SpotifyAPI.Requests;

namespace SpotifyAPI.Validations
{
    public class PasswordResetRequestValidator : AbstractValidator<PasswordResetRequest>
    {
        public PasswordResetRequestValidator()
        {
            RuleFor(requestBody => requestBody.Login)
            .Cascade(CascadeMode.Stop)
            .Login();
        }
    }
}