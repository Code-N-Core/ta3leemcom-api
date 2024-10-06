using Microsoft.AspNetCore.Identity;
using PlatformAPI.Core.Const;
using PlatformAPI.Core.DTOs.Questions;
using PlatformAPI.Core.DTOs.Quiz;
using PlatformAPI.Core.Interfaces;
using PlatformAPI.Core.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatformAPI.Core.Services
{
    public class QuizService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly QuestionService questionService;

        public QuizService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, QuestionService questionService)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            this.questionService = questionService;
        }
        public async Task<List<Quiz>> GetEndedQuizez()
        {
            var datenow = DateTime.Now;

            var endedQuizzes = await _unitOfWork.Quiz.GetEndedQuiz(datenow);
     
            // Now manually filter in memory to debug and inspect
            var d = endedQuizzes.Last().EndDate;
            var filteredQuizzes = endedQuizzes
            .Where(q => q.EndDate <= datenow)  // Client-side evaluation of the date logic
                .ToList();
            return filteredQuizzes;
        }

        public async Task<StudentQuiz> CreateQuizStudent(Quiz quiz, StudentSolutionDTO model,int totalmark, int bouncemark)
        {
            StudentQuiz sq = new StudentQuiz
            {
                QuizId = model.QuizId,
                StudentId = model.StudentId,
                IsAttend = model.SubmitAnswersDate <= quiz.EndDate ? true : false,
                StudentMark = totalmark,
                SubmitAnswerDate=model.SubmitAnswersDate,
                StudentBounce=bouncemark
            };
            await _unitOfWork.StudentQuiz.AddAsync(sq);
            await _unitOfWork.CompleteAsync();
            return sq;
        }
        public async Task<IOrderedEnumerable<StudentQuizResultDTO>> GetAllStudentQuizResults(IEnumerable<StudentQuiz> studentQuizzes)
        {
            List<StudentQuizResultDTO> quizResults = new List<StudentQuizResultDTO>();

            foreach (var sq in studentQuizzes)
            {
                var student=await _unitOfWork.Student.GetByIdAsync(sq.StudentId);
                var result = new StudentQuizResultDTO
                {
                    Id = sq.Id,
                    QuizId=sq.QuizId,
                    StuentName = _userManager.FindByEmailAsync(student.Code + StudentConst.EmailComplete).Result.Name, // Assuming Student has a Name property
                    StudentMark = sq.StudentMark,
                    StudentBounce = sq.StudentBounce,
                    QuizMark = sq.Quiz.Mark,
                    QuizBounce = sq.Quiz.Bounce,
                    IsAttend = sq.IsAttend,
                    SubmitAnswerTime = sq.SubmitAnswerDate,
                    Answers=new List<StudentAnswerDTO> { }
                };
                var anss = await _unitOfWork.StudentAnswer.GetStudentAnswers(sq.Id);
                foreach (var ans in anss)
                {
                    var answer = new StudentAnswerDTO
                    {
                        id = ans.Id,
                        iscorrect = ans.IsCorrect,
                        questionId = ans.QuestionId,
                        QuestionMark=ans.Question.Mark,
                        ChoiceId=ans.ChosenOptionId
                    };
                    result.Answers.Add(answer);
                }

                quizResults.Add(result);
            }

            return quizResults.OrderByDescending(x=>x.StudentMark+x.StudentBounce);
        }
        public async Task<ShowQuizDescreptionDTO> GetQuizDescreption(int quizId)
        {
            if (quizId == 0) return null;

                var quiz = await _unitOfWork.Quiz.GetByIdAsync(quizId);
            var questionsCount = await _unitOfWork.Question.GetNumbersOfQuestionInQuizId(quizId);
            var studentsNotEntered = await _unitOfWork.Student.GetStudentNotEnter(quizId);

            var results = new ShowQuizDescreptionDTO
            {
                id = quizId,
                Title = quiz.Title,
                Mark = quiz.Mark,
                Bounce = quiz.Bounce.HasValue ? quiz.Bounce.Value : 0,
                NumQuestions = questionsCount,
                NumStuedntsSolveQuiz = await _unitOfWork.Student.GetAllStudentsSolveQuiz(quizId),
                NumStuedntsNotSolveQuiz = studentsNotEntered.Count(),
            };
          
            return results;
        }
        public DateTime GetDateTimeFromTimeStart(timeStart time,DateTime date)
        {
            // Convert the 12-hour format to 24-hour format based on AM/PM mode
            int hoursIn24Format = time.Hours;

            if (time.Mode.Equals("PM", StringComparison.OrdinalIgnoreCase) && time.Hours != 12)
            {
                hoursIn24Format += 12;
            }
            else if (time.Mode.Equals("AM", StringComparison.OrdinalIgnoreCase) && time.Hours == 12)
            {
                hoursIn24Format = 0; // Midnight
            }

            // Create a DateTime object for today's date with the given time
            DateTime result = new DateTime(
                date.Year,
                date.Month,
                date.Day,
                hoursIn24Format,
                time.Minute,
                0 // Seconds are set to zero by default
            );

            return result;
        }

        public async Task deleteQuiz(int quizId)
        {
            var quiz = await _unitOfWork.Quiz.FindTWithIncludes<Quiz>(quizId,
                  q => q.Questions,
                  q => q.GroupsQuizzes,
                  q => q.StudentsQuizzes
                  );
            foreach (var question in quiz.Questions)
            {
                await questionService.DeleteQuestionsWithChoises(question.Id);
            }
            foreach (var gq in quiz.GroupsQuizzes)
            {
                await _unitOfWork.GroupQuiz.DeleteAsync(gq);
            }
            foreach (var sq in quiz.StudentsQuizzes)
            {
                await _unitOfWork.StudentQuiz.DeleteAsync(sq);
            }
            await _unitOfWork.CompleteAsync();
            await _unitOfWork.Quiz.DeleteAsync(quiz);
            await _unitOfWork.CompleteAsync();
        }

    }
}
