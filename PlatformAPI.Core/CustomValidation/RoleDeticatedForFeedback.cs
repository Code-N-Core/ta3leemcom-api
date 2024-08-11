using PlatformAPI.Core.Const;

namespace PlatformAPI.Core.CustomValidation
{
    public class RoleDeticatedForFeedback:ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult("null value");
            }
            if (value.ToString() == Roles.Teacher.ToString() || value.ToString() == Roles.Parent.ToString()||value.ToString()==Roles.Student.ToString())
                return ValidationResult.Success;
            return new ValidationResult("should only Teacher or Parent or Student!");
        }
    }
}
