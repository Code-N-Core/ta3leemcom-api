namespace PlatformAPI.Core.CustomValidation
{
    public class StarsDeticated: ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult("null value");
            }
            if (int.Parse(value.ToString())<=5&& int.Parse(value.ToString())>=1)
                return ValidationResult.Success;
            return new ValidationResult("invalid number of stars!");
        }
    }
}
