namespace PlatformAPI.Core.CustomValidation
{
    public class BasedOnTimeExist:ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult("null value");
            }
            //if(value)

            return new ValidationResult("");
        }
    }
}
