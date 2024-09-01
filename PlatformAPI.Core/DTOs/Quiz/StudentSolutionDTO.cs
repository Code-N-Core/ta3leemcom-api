using PlatformAPI.Core.DTOs.Questions;

namespace PlatformAPI.Core.DTOs.Quiz
{
    public class StudentSolutionDTO
    {
        public int QuizId { get; set; }
        public int StudentId { get; set; }

       public List<QuestionForm> questionForms {  get; set; }
        public DateTime SubmitAnswersDate {  get; set; }

    }
}
