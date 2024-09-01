using System.ComponentModel.DataAnnotations.Schema;

namespace PlatformAPI.Core.DTOs.StudentAbsence
{
    public class StudentAbsenceDTO
    {
        public int StudentId { get; set; }
        public int DayId { get; set; }
        public bool Attended { get; set; }
    }
}