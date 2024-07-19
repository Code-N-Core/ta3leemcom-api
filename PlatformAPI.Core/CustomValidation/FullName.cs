using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatformAPI.Core.CustomValidation
{
    public class FullName : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult("null value");
            }
            string name = value.ToString();
            string[] arr = name.Split(' ');
            if(arr.Length >= 3 )
                return ValidationResult.Success;

            return new ValidationResult("يجب ان يكون الاسم علي الاقل ثلاثي");
        }
    }
}
