using PlatformAPI.Core.Models;

namespace PlatformAPI.Core.DTOs.Day
{
    public class DayDTO
    {
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public int MonthId { get; set; }
    }
}