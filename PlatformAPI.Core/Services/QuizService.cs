using AutoMapper;
using Microsoft.AspNetCore.Identity;
using PlatformAPI.Core.Const;
using PlatformAPI.Core.DTOs.Choose;
using PlatformAPI.Core.DTOs.Questions;
using PlatformAPI.Core.DTOs.Quiz;
using PlatformAPI.Core.Helpers;
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
        private readonly IMapper _mapper;
        private readonly IImageService _imageService;
        private const string ImagesFolderForQuestions = "uploads/Questions";
        private const string ImagesFolderForChioces = "uploads/Chioces";
        public QuizService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, QuestionService questionService, IMapper mapper, IImageService imageService)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            this.questionService = questionService;
            _mapper = mapper;
            _imageService = imageService;
        }
        public async Task<ShowQuiz> CreateOnlineQuiz(CreateOnlineQuizDTO model)
        {
            try
            {
                // Map the quiz DTO to the quiz entity
                var quiz = _mapper.Map<Quiz>(model);


                // Assign StartDate and Duration
                quiz.Duration = new TimeSpan(model.timeDuration.Days, model.timeDuration.Hours, model.timeDuration.Minute, 0);
                quiz.Questions = new List<Question>();

                foreach (var questionDto in model.Questions)
                {
                    var question = _mapper.Map<Question>(questionDto);
                    question.Chooses = new List<Choose>();

                    if (questionDto.AttachFile != null)
                    {

                        question.attachmentPath = _imageService.SaveImage(questionDto.AttachFile, ImagesFolderForQuestions);
                    }
                    foreach (var choicedto in questionDto.Choices)
                    {
                        var Choice = _mapper.Map<Choose>(choicedto);

                        if (choicedto.AttachFile != null)
                        {
                            Choice.attachmentPath = _imageService.SaveImage(choicedto.AttachFile, ImagesFolderForChioces);
                        }
                        Choice.Question = question;
                        question.Chooses.Add(Choice);
                    }
                    question.Quiz = quiz;
                    quiz.Questions.Add(question);

                }
                await _unitOfWork.Quiz.AddAsync(quiz);
                await _unitOfWork.CompleteAsync(); // Commit after adding quiz


                // Create GroupQuiz entries
                var shq = _mapper.Map<ShowQuiz>(quiz);
                shq.GroupsIds = new List<int>();


                foreach (var group in model.GroupsIds)
                {
                    var groupQuiz = new GroupQuiz
                    {
                        GroupId = group,
                        QuizId = quiz.Id,
                    };
                    await _unitOfWork.GroupQuiz.AddAsync(groupQuiz);
                    shq.GroupsIds.Add(groupQuiz.GroupId);
                }

                await _unitOfWork.CompleteAsync(); // Commit after adding group quiz



                // Retrieve and map all questions
                shq.questionsOfQuizzes = new List<ShowQuestionsOfQuiz>(await questionService.GetAllQuestionsOfQuiz(quiz.Id, true));
                return shq;
            }
            catch (Exception ex)
            {

                throw ex;
            }
          
        }
        public async Task<List<Quiz>> GetEndedQuizez()
        {
            // Define Egypt's time zone
            TimeZoneInfo egyptTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");

            // Get the current time in Egypt
            DateTime datenow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, egyptTimeZone);

            var endedQuizzes = await _unitOfWork.Quiz.GetEndedQuiz(datenow);
     
            // Now manually filter in memory to debug and inspect
            var filteredQuizzes = endedQuizzes
            .Where(q => q.EndDate <= datenow)  // Client-side evaluation of the date logic
                .ToList();
            if (!filteredQuizzes.Any())
                return null;
            return filteredQuizzes;
        }

        public async Task<StudentQuiz> CreateQuizStudent(Quiz quiz, StudentSolutionDTO model,int totalmark, int bouncemark)
        {
            StudentQuiz sq = new StudentQuiz
            {
                QuizId = model.QuizId,
                StudentId = model.StudentId,
                IsAttend = model.submitAnswersDateTime <= quiz.EndDate ? true : false,
                StudentMark = totalmark,
                SubmitAnswerDate=model.submitAnswersDateTime,
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
            var qustionsDes= (await _unitOfWork.Question.GetAllAsync()).Where(q=>q.QuizId==quizId).Select(q=>new QuestionsDes {id=q.Id,Type= q.Type,Mark=q.Mark }).ToList();

            var results = new ShowQuizDescreptionDTO
            {
                id = quizId,
                Title = quiz.Title,
                Mark = quiz.Mark,
                Bounce = quiz.Bounce.HasValue ? quiz.Bounce.Value : 0,
                NumQuestions = questionsCount,
                NumStuedntsSolveQuiz = await _unitOfWork.Student.GetAllStudentsSolveQuiz(quizId),
                NumStuedntsNotSolveQuiz = studentsNotEntered.Count(),
                QuestionsDes= qustionsDes
            };
          
            return results;
        }
/*        public DateTime GetDateTimeFromTimeStart(timeStart time,DateTime date)
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
*/
        public async Task deleteQuiz(int quizId)
        {
            try
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
                var tecnot = await _unitOfWork.Notification.GetAllNots(quiz.TeacherId);
                if (tecnot != null)
                {
                    foreach (var not in tecnot)
                    {
                        if (not.quizId == quizId)
                        {
                            var noti = await _unitOfWork.Notification.GetByIdAsync(not.NotificationId);
                            await _unitOfWork.Notification.DeleteAsync(noti);
                        }
                    }
                }
                await _unitOfWork.Quiz.DeleteAsync(quiz);
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception ex)
            {

                throw ex;
            }
         
        }
        public async Task<ShowQuiz> CreateGroupsOfQuiz(Quiz quiz,CreateOnlineQuizDTO model)
        {


            // Add the quiz to the repository
            var shq = _mapper.Map<ShowQuiz>(quiz);
            shq.GroupsIds = new List<int>();


            foreach (var group in model.GroupsIds)
            {
                var groupQuiz = new GroupQuiz
                {
                    GroupId = group,
                    QuizId = quiz.Id,
                };
                await _unitOfWork.GroupQuiz.AddAsync(groupQuiz);
                shq.GroupsIds.Add(groupQuiz.GroupId);
            }

            await _unitOfWork.CompleteAsync(); // Commit after adding group quiz



            // Retrieve and map all questions
            shq.questionsOfQuizzes = new List<ShowQuestionsOfQuiz>(await questionService.GetAllQuestionsOfQuiz(quiz.Id, true));
            return shq;
        }

    }
}
