namespace PlatformAPI.Core.Models
{
    public class Month
    {
        [Key]
        public int Id { get; set; }
        [Required,MinLength(1),MaxLength(30)]
        public string Name { get; set; }
        public virtual List<Day> Days { get; set; }
        public virtual List<StudentMonth> StudentMonths { get; set; }
        [Required]
        public int GroupId { get; set; }
        public virtual Group Group { get; set; }
        [Required]
        public int Year { get; set; }
    }
}