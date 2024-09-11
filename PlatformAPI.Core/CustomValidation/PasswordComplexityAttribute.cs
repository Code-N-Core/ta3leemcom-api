namespace PlatformAPI.Core.CustomValidation
{
    public class PasswordComplexityAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var password = value as string;

            if (string.IsNullOrEmpty(password))
                return new ValidationResult("كلمة السر مطلوبة.");

            // Check minimum length
            if (password.Length < 8)
                return new ValidationResult("كلمة السر لازم تكون 8 حروف على الأقل.");

            // Check for digit
            if (!password.Any(char.IsDigit))
                return new ValidationResult("كلمة السر لازم يكون فيها رقم واحد على الأقل.");

            // Check for non-alphanumeric character
            if (!password.Any(ch => !char.IsLetterOrDigit(ch)))
                return new ValidationResult("كلمة السر لازم يكون فيها رمز خاص واحد على الأقل.");

            // Check for uppercase letter
            if (!password.Any(char.IsUpper))
                return new ValidationResult("كلمة السر لازم يكون فيها حرف كابيتال واحد على الأقل.");

            // Check for lowercase letter
            if (!password.Any(char.IsLower))
                return new ValidationResult("كلمة السر لازم يكون فيها حرف صغير واحد على الأقل.");

            return ValidationResult.Success;
        }
    }

}