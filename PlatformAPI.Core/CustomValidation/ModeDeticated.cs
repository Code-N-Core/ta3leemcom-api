using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatformAPI.Core.CustomValidation
{
    public class ModeDeticated : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult("null value");
            }
            if (value.ToString() == "AM" || value.ToString() == "PM")
                return ValidationResult.Success;
            return new ValidationResult("should only AM or PM!");
        }
    }
}
