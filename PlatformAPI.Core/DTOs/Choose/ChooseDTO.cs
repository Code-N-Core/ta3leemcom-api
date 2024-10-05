namespace PlatformAPI.Core.DTOs.Choose
{
    public class ChooseDTO
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public bool? IsCorrect { get; set; }
        public int? QuestionId { get; set; }
        public bool IsDeleted { get; set; }

    }
}
