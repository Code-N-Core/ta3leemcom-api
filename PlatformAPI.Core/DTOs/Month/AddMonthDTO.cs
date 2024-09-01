using PlatformAPI.Core.CustomValidation;

namespace PlatformAPI.Core.DTOs.Month
{
    public class AddMonthDTO
    {
        [Required,AvailableMonth]
        public string Name { get; set; }
        [Required]
        public int Year { get; set; }
        [Required]
        public int GroupId { get; set; }
    }
}