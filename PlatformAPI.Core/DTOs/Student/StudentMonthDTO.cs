using PlatformAPI.Core.DTOs.StudentAbsence;

namespace PlatformAPI.Core.DTOs.Student
{
    public class StudentMonthDTO
    {
        public int StudentId { get; set; }
        public string Name { get; set; }
        public List<StudentAbsenceForMonthDTO> StudentAbsences { get; set; }
        public bool Pay { get; set; }
    }
}