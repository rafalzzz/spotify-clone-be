using FluentValidation;
using SpotifyAPI.Extensions;
using SpotifyAPI.Requests;

namespace SpotifyAPI.Validations
{
    public class PasswordResetCompleteRequestValidator : AbstractValidator<PasswordResetCompleteRequest>
    {
        public PasswordResetCompleteRequestValidator()
        {
            RuleFor(requestBody => requestBody.Password)
            .Cascade(CascadeMode.Stop)
            .Password();
        }
    }
}