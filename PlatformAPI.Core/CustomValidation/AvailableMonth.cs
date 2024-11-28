namespace PlatformAPI.Core.CustomValidation
{
    public class AvailableMonth:ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult("null value");
            }
            if (value.ToString() == "يناير" || value.ToString() == "فبراير" 
                || value.ToString() == "مارس"|| value.ToString() == "أبريل"
                || value.ToString() == "مايو"|| value.ToString() == "يونيو"
                || value.ToString() == "يوليو" || value.ToString() == "أغسطس"
                || value.ToString() == "سبتمبر" || value.ToString() == "أكتوبر"
                || value.ToString() == "نوفمبر" || value.ToString() == "ديسمبر")
                return ValidationResult.Success;
            return new ValidationResult("Invalid Name!");
        }
    }
}
