using PlatformAPI.Core.DTOs.StudentAbsence;

namespace PlatformAPI.Core.DTOs.Day
{
    public class ViewDayDTO
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public int MonthId { get; set; }
        public IEnumerable<StudentAbsenceDTO>? studentAbsences { get; set; }
    }
}
