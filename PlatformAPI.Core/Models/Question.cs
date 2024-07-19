namespace PlatformAPI.Core.Models
{
    public class Question
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int Mark { get; set; }
        [Required,MinLength(5),MaxLength(1000)]
        public string Content { get; set; }
        [Required, MinLength(1), MaxLength(1000)]
        public string Answer { get; set; }
        [Required, MinLength(1), MaxLength(1000)]
        public string Explain { get; set; }
        [Required]
        public int QuizId { get; set; }
        public virtual Quiz Quiz { get; set; }
        public virtual List<Choose> Chooses { get; set; }
    }
}
