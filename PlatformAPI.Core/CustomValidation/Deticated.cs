namespace PlatformAPI.Core.CustomValidation
{
    public class Deticated:ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is int price && price >= 0)
            {
                return ValidationResult.Success;
            }
            return new ValidationResult("The value must be greater than or equal to 0.");
        }
    }
}