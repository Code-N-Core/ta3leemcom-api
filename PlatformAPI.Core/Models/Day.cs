namespace PlatformAPI.Core.Models
{
    public class Day
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public int MonthId { get; set; }
        public virtual Month Month { get; set; }
        public virtual List<StudentAbsence> StudentAbsence { get; set; }
    }
}
