using PlatformAPI.Core.DTOs.Day;
using PlatformAPI.Core.DTOs.StudentAbsence;
using PlatformAPI.Core.DTOs.StudentMonth;

namespace PlatformAPI.Core.DTOs.Month
{
    public class SaveMonthDataDTO
    {
        [Required]
        public IEnumerable<StudentAbsenceDTO> AbsenceStudents { get; set; }
        [Required]
        public IEnumerable<StudentMonthDto> MonthStudents { get; set; }
    }
}