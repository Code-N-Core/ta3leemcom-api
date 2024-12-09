using System;

namespace PlatformAPI.Core.DTOs.Quiz
{
    public class QuizStatusDto
    {
        public int QuizId { get; set; }                     // Unique identifier for the quiz
        public string Title {  get; set; }
        public int? StudentQuizId { get; set; }                     // Unique identifier for the studentquiz
        public DateTime StartDate { get; set; }              // Start date and time of the quiz
        public DateTime EndDate { get; set; }                // End date and time of the quiz
        public bool? IsAttend { get; set; }                   // Indicates if the student attended the quiz
        public DateTime? SubmitAnswerDate { get; set; }     // Nullable to represent that the answer might not be submitted
        public string QuizStatus { get; set; }               // Status of the quiz (e.g., Not Started, Available, Ended, Solved)
        public string SolveStatus { get; set; }              // Status of solving the quiz (e.g., Solved In Time, Solved Late, Not Solved)
        public TimeSpan Duration { get; set; }               // Duration of the quiz as a TimeSpan
        public int MandatoryQuestionCount { get; set; }      // Count of mandatory questions in the quiz
        public int OptionalQuestionCount { get; set; }       // Count of optional questions in the quiz
        public int TotalMark { get; set; }                   // Total marks for the quiz
        public int? StudentMark { get; set; }                // Nullable student mark (if applicable)
        public int? Bounce { get; set; }                     // Nullable property to hold a bounce value (if needed)
        public int? StudentBounce { get; set; }              // Nullable property for student-specific bounce value (if needed)
        public int? OrderOfStudent { get; set; }              // Nullable property for student-specific bounce value (if needed)
    }
}
