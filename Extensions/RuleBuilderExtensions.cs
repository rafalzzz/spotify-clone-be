using FluentValidation;
using SpotifyAPI.Enums;

namespace SpotifyAPI.Extensions
{
    public static class RuleBuilderExtensions
    {
        public static void FirstName<T>(this IRuleBuilder<T, string> ruleBuilder, int minimumLength = 2, int maxLength = 150)
        {
            ruleBuilder
            .NotEmpty()
            .WithMessage("First name is required")
            .MinimumLength(minimumLength)
            .WithMessage($"First name must contain at least {minimumLength} characters")
            .MaximumLength(maxLength)
            .WithMessage($"First name can contain up to {maxLength} characters")
            .Matches("^[A-Z]")
            .WithMessage("First name must start with an uppercase letter")
            .Matches("^[a-zA-Z]")
            .WithMessage("First name can only contain letters");
        }

        public static void LastName<T>(this IRuleBuilder<T, string> ruleBuilder, int minimumLength = 2, int maxLength = 150)
        {
            ruleBuilder
            .NotEmpty()
            .WithMessage("Last name is required")
            .MinimumLength(minimumLength)
            .WithMessage($"Last name must contain at least {minimumLength} characters")
            .MaximumLength(maxLength)
            .WithMessage($"Last name can contain up to {maxLength} characters")
            .Matches("^[A-Z]")
            .WithMessage("Last name must start with an uppercase letter")
            .Matches("^([A-Z][a-z]*)(-[A-Z][a-z]*)?$")
            .WithMessage("Last name must only contain letters and may consist of two parts separated by a pause mark");
        }

        public static void Nickname<T>(this IRuleBuilder<T, string> ruleBuilder, int minimumLength = 2, int maxLength = 150)
        {
            ruleBuilder
            .NotEmpty()
            .WithMessage("Nickname is required")
            .MinimumLength(minimumLength)
            .WithMessage($"Nickname must contain at least {minimumLength} characters")
            .MaximumLength(maxLength)
            .WithMessage($"Nickname can contain up to {maxLength} characters")
            .Matches("^[a-zA-Z0-9]+$")
            .WithMessage("Nickname must only contain letters and digits");
        }

        public static void Email<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            ruleBuilder
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Invalid email address");
        }

        public static void Password<T>(this IRuleBuilder<T, string> ruleBuilder, int minimumLength = 8, int maxLength = 150)
        {
            ruleBuilder
            .MinimumLength(minimumLength)
            .WithMessage($"Password must contain at least {minimumLength} characters")
            .MaximumLength(maxLength)
            .WithMessage($"Password can contain up to {maxLength} characters")
            .Matches("[a-z]")
            .WithMessage("Password must contain at least one lowercase letter")
            .Matches("[A-Z]")
            .WithMessage("Password must contain at least one uppercase letter")
            .Matches("[0-9]")
            .WithMessage("Password must contain at least one digit")
            .Matches("[^a-zA-Z0-9]")
            .WithMessage("Password must contain at least one special character");
        }

        public static void Token<T>(this IRuleBuilder<T, string> ruleBuilder, int minimumLength = 20)
        {
            ruleBuilder
            .MinimumLength(minimumLength)
            .WithMessage($"Token must contain at least {minimumLength} characters");
        }

        public static void Id<T>(this IRuleBuilder<T, int> ruleBuilder)
        {
            ruleBuilder
            .NotEmpty()
            .WithMessage("ID is required")
            .Must(id => id > 0)
            .WithMessage("ID must be a positive number");
        }

        public static void Gender<T>(this IRuleBuilder<T, UserGender> ruleBuilder)
        {
            ruleBuilder
            .NotNull()
            .WithMessage("Gender is required")
            .IsInEnum()
            .WithMessage("Invalid gender value. It can be only 'Male', 'Female' or 'Other'");
        }

        public static void Prefix<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            ruleBuilder
            .NotNull()
            .WithMessage("Phone number prefix is required")
            .Matches(@"\+\d{2}")
            .WithMessage("Invalid prefix format. It must be '+XX' where X represents digits");
        }

        public static void PhoneNumber<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            ruleBuilder
            .NotNull()
            .WithMessage("Phone number is required")
            .Matches(@"^\d{9}$")
            .WithMessage("Invalid phone number");
        }
    }
}