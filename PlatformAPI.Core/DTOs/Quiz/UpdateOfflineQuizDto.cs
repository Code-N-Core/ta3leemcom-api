namespace PlatformAPI.Core.DTOs.Quiz
{
    public class UpdateOfflineQuizDto : CreateOffLineQuizDto
    {
        [Key]
        public int Id { get; set; }
    }
}
