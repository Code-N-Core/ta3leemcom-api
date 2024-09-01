using PlatformAPI.Core.DTOs.Day;
using PlatformAPI.Core.DTOs.StudentMonth;

namespace PlatformAPI.Core.DTOs.Month
{
    public class ViewMonthDTO
    {
        public int Id { get; set; }
        [Required, MinLength(1), MaxLength(30)]
        public string Name { get; set; }
        public int GroupId { get; set; }
        public int Year { get; set; }
        public IEnumerable<ViewDayDTO>? Days { get; set; }
        public IEnumerable<StudentMonthDto>? MonthStudents { get; set; }
    }
}