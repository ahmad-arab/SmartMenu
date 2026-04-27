using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace SmartMenu.Attributes.Validation
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public sealed partial class DomainNameAttribute : ValidationAttribute
    {
        [GeneratedRegex("^[a-zA-Z0-9-]+$")]
        private static partial Regex DomainLabelRegex();

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
            if (candidate.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                candidate.StartsWith("https://", StringComparison.OrdinalIgnoreCase) ||
                candidate.Contains('/') ||
                candidate.Contains(' '))
            {
                return new ValidationResult(ErrorMessage ?? "Please enter a valid domain or subdomain name.");
            }

            if (candidate.EndsWith('.'))
            {
                candidate = candidate[..^1];
            }

            if (candidate.Length == 0 || candidate.Length > 253)
            {
                return new ValidationResult(ErrorMessage ?? "Please enter a valid domain or subdomain name.");
            }

            if (Uri.CheckHostName(candidate) != UriHostNameType.Dns)
            {
                return new ValidationResult(ErrorMessage ?? "Please enter a valid domain or subdomain name.");
            }

            var labels = candidate.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (labels.Length < 2)
            {
                return new ValidationResult(ErrorMessage ?? "Please enter a valid domain or subdomain name.");
            }

            foreach (var label in labels)
            {
                if (label.Length == 0 ||
                    label.Length > 63 ||
                    label.StartsWith('-') ||
                    label.EndsWith('-') ||
                    !DomainLabelRegex().IsMatch(label))
                {
                    return new ValidationResult(ErrorMessage ?? "Please enter a valid domain or subdomain name.");
                }
            }

            return ValidationResult.Success;
        }
    }
}
