namespace PlatformAPI.Core.CustomValidation
{
    public class Deticated:ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult("null value");
            }
            if (value.ToString() == "اونلاين" || value.ToString() == "اوفلاين")
                return ValidationResult.Success;
            return new ValidationResult("should only online or offline!");
        }
    }
}