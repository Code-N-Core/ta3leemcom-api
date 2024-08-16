using System;
using System.Collections.Generic;
namespace PlatformAPI.Core.DTOs
{
    public class ContactDTO
    {
        [Required,MaxLength(50)]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required,MaxLength(50)]
        public string Subject { get; set; }
        [Required,MaxLength(2000)]
        public string Message { get; set; }
    }
}