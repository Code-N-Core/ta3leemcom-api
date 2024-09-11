namespace PlatformAPI.Core.DTOs.Parent
{
    public class StudentMonthParentDTO
    {
        public string MonthName { get; set; }
        public int Year { get; set; }
        public bool Pay { get; set; }
        public IEnumerable<StudentMonthDayParentDTO> Days { get; set; }
    }
}