using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatformAPI.Core.CustomValidation
{
    public class AllowedExtensionsAttribute : ValidationAttribute
    {
        private readonly string _allowedExtensions;

        public AllowedExtensionsAttribute(string allowedExtensions)
        {
            _allowedExtensions = allowedExtensions;
        }
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var file = value as IFormFile;
            if (file != null)
            {
                var ext = Path.GetExtension(file.FileName);
                var isallowed = _allowedExtensions.Split(",").Contains(ext, StringComparer.OrdinalIgnoreCase);
                if (!isallowed)
                {
                    return new ValidationResult($"Only {_allowedExtensions} are allwoed");
                }
            }
            return ValidationResult.Success;
        }
    }
}
