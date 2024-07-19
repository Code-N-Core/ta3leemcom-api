namespace PlatformAPI.Core.Models
{
    public class GroupQuiz
    {
        public int GroupId { get; set; }
        public virtual Group Group { get; set; }
        public int QuizId { get; set; }
        public virtual Quiz Quiz { get; set; }
    }
}