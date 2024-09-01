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
                || value.ToString() == "مارس"|| value.ToString() == "ابريل"
                || value.ToString() == "مايو"|| value.ToString() == "يونيو"
                || value.ToString() == "يوليو" || value.ToString() == "اغسطس"
                || value.ToString() == "سبتمبر" || value.ToString() == "اكتوبر"
                || value.ToString() == "نوفمبر" || value.ToString() == "ديسمبر")
                return ValidationResult.Success;
            return new ValidationResult("Invalid Name!");
        }
    }
}
