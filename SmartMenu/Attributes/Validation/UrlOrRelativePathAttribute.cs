using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Attributes.Validation
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public sealed class UrlOrRelativePathAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            if (value is not string input || string.IsNullOrWhiteSpace(input))
            {
                return ValidationResult.Success;
            }

            var candidate = input.Trim();

            if (Uri.TryCreate(candidate, UriKind.Absolute, out var absoluteUri))
            {
                if (absoluteUri.Scheme == Uri.UriSchemeHttp || absoluteUri.Scheme == Uri.UriSchemeHttps)
                {
                    return ValidationResult.Success;
                }

                return new ValidationResult(ErrorMessage ?? "Please enter a valid URL.");
            }

            if (candidate.StartsWith("//", StringComparison.Ordinal) ||
                candidate.Contains("..", StringComparison.Ordinal) ||
                candidate.Contains(' '))
            {
                return new ValidationResult(ErrorMessage ?? "Please enter a valid URL.");
            }

            return Uri.TryCreate(candidate, UriKind.Relative, out _)
                ? ValidationResult.Success
                : new ValidationResult(ErrorMessage ?? "Please enter a valid URL.");
        }
    }
}
