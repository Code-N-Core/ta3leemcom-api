namespace PlatformAPI.Core.DTOs.StudentMonth
{
    public class StudentMonthDto
    {
        [Required]
        public int StudentId { get; set; }
        [Required]
        public int MonthId { get; set; }
        [Required]
        public bool Pay { get; set; }
    }
}