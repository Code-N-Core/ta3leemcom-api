﻿using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using PlatformAPI.Core.DTOs.Choose;
using PlatformAPI.Core.DTOs.Questions;
using PlatformAPI.Core.DTOs.Quiz;
using PlatformAPI.Core.Helpers;
using PlatformAPI.Core.Models;
using PlatformAPI.Core.Services;
using System.Security.Claims;
using System.Transactions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace PlatformAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly QuestionService questionService;
        private readonly ChooseService chooseService;
        private readonly QuizService QuizService;
       



        public QuizController(IUnitOfWork unitOfWork, IMapper mapper, QuestionService questionService,
            ChooseService chooseService, QuizService QuizService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            this.questionService = questionService;
            this.chooseService = chooseService;
            this.QuizService = QuizService;
        }
        [Authorize(Roles= "Teacher")]
        [HttpGet("GetAllQuizsByGroupsIds")]
        public async Task<IActionResult> GetAllByGroup([FromQuery]List<int> GroupsIds)
        {
            // Get the logged-in teacher's ID from the token
            var loggedInTeacherId = User.FindFirst("LoggedId")?.Value;

            if (string.IsNullOrEmpty(loggedInTeacherId))
            {
                return Unauthorized("User not found");
            }
            var groups = (await _unitOfWork.Group.GetAllAsync()).Where(g => GroupsIds.Contains(g.Id));
            if (groups == null)
            {
                return NotFound("Group not found");
            }

            if (groups.Any(g=>g.TeacherId != int.Parse(loggedInTeacherId)))
            {
                return BadRequest("You do not have permission to get These groups");
            }


            var quizs =await _unitOfWork.Quiz.GetQuizzesByGroupsIds(GroupsIds);
            if (quizs == null) return NotFound($"There is No Quizs For These Groups");
            List<ShowQuiz> lsq = new List<ShowQuiz>();
            foreach (var quiz in quizs) 
            {
                var sq=_mapper.Map<ShowQuiz>(quiz);
                lsq.Add(sq);
            }
            return Ok(lsq);
        }
        [Authorize(Roles= "Teacher,Student")]
        [HttpGet("GetQuizById")]
        public async Task<IActionResult>GetById(int QuizId)
        {
            // Get the logged-in teacher's ID from the token
            var loggedInId = User.FindFirst("LoggedId")?.Value;
            var userRoles = User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToList();
            bool f=false;
            if (userRoles.Contains(Roles.Teacher.ToString()))
            {
                if (string.IsNullOrEmpty(loggedInId))
                {
                    return Unauthorized("User not found");
                }
                var quiz = (await _unitOfWork.Quiz.GetByIdAsync(QuizId));
                if (quiz == null)
                {
                    return NotFound("Quiz not found");
                }

                if (quiz.TeacherId != int.Parse(loggedInId))
                {
                    return BadRequest("You do not have permission to get this Quiz");
                }
                else
                {
                    f = true;
                }
            }
            else if (userRoles.Contains(Roles.Student.ToString()))
            {
                var groupofstudent = (await _unitOfWork.Student.GetByIdAsync(int.Parse(loggedInId))).GroupId;
                var groupIdsOfQuiz=(await _unitOfWork.GroupQuiz.GetAllAsync()).Where(gq=>gq.QuizId==QuizId).Select(gq=>gq.GroupId).ToList();
                if (!groupIdsOfQuiz.Contains(groupofstudent))
                {
                    return BadRequest("You do not have permission to get this Quiz");
                }
                else
                {
                    f = false;
                }
            }

            //////////////////////////
            ///
            /// // Define Egypt's time zone
            TimeZoneInfo egyptTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");

            // Get the current time in Egypt
            DateTime datenow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, egyptTimeZone);

            var quizs =await _unitOfWork.Quiz.FindTWithIncludes<Quiz>(QuizId,q=>q.GroupsQuizzes);
            if (quizs == null) return NotFound($"There is No Quizs With Id {QuizId}");
            var sq=_mapper.Map<ShowQuiz>(quizs);
            sq.GroupsIds=new List<int> ();
            foreach (var gq in quizs.GroupsQuizzes)
            {
                sq.GroupsIds.Add(gq.GroupId);
            }
            if(quizs.EndDate< datenow)
                f=true;
            sq.questionsOfQuizzes =await questionService.GetAllQuestionsOfQuiz(QuizId,f);
            var IsHeSolveQuiz =await _unitOfWork.StudentQuiz.FindTWithExpression<StudentQuiz>(sq => sq.StudentId == int.Parse(loggedInId) && sq.QuizId == QuizId);
            if (f == false && IsHeSolveQuiz != null)
            {
                sq.isAttending = true;   
            }
            return Ok(sq);
        }
        [Authorize(Roles ="Teacher")]
        [HttpGet("GetAllResultsOfQuizId")]
        public async Task<IActionResult> GetResults(int quizId)
        {
            var loggedInId = User.FindFirst("LoggedId")?.Value;
            if (string.IsNullOrEmpty(loggedInId))
            {
                return Unauthorized("User not found");
            }
            var quiz = (await _unitOfWork.Quiz.GetByIdAsync(quizId));
            if (quiz == null)
            {
                return NotFound("Quiz not found");
            }

            if (quiz.TeacherId != int.Parse(loggedInId))
            {
                return BadRequest("You do not have permission to get this Quiz");
            }

            if (quizId == 0) return BadRequest($"There is no Quiz With ID: {quizId}");
            var studentQuizzes = await _unitOfWork.StudentQuiz
                .FindAllWithIncludes<StudentQuiz>(q => q.QuizId == quizId ,
                    Sq => Sq.Quiz);
            var res=await QuizService.GetAllStudentQuizResults(studentQuizzes);
            if (res == null) return BadRequest();

            return Ok(res);
        }

        [Authorize(Roles="Student,Teacher")]
        [HttpGet("GetStudentSolutionByStudentQuizId")]
        public async Task<IActionResult> GetResultOfStudentQuizId(int studentQuizId)
        {
            var studentQuiz = await _unitOfWork.StudentQuiz.FindTWithIncludes<StudentQuiz>(studentQuizId,
                sq=>sq.Quiz,
                sq=>sq.StudentAnswers);

            var studentId = studentQuiz.StudentId;

           
            var lsq=new List<StudentQuiz>();
            lsq.Add(studentQuiz);
            var res = (await QuizService.GetAllStudentQuizResults(lsq)).FirstOrDefault();
            if (res == null) return BadRequest();


            var quiz = await _unitOfWork.Quiz.GetByIdAsync(studentQuiz.QuizId);
            if (quiz == null) return NotFound($"There is No Quizs With Id {quiz.Id}");

            var loggedInId = User.FindFirst("LoggedId")?.Value;
            var userRoles = User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToList();
            if (userRoles.Contains(Roles.Teacher.ToString()))
            {
                if (string.IsNullOrEmpty(loggedInId))
                {
                    return Unauthorized("User not found");
                }               

                if (quiz.TeacherId != int.Parse(loggedInId))
                {
                    return BadRequest("You do not have permission to get this Quiz");
                }
               
            }
            else if (userRoles.Contains(Roles.Student.ToString()))
            {
                if(studentId!=int.Parse(loggedInId))
                    return BadRequest("You do not have permission to get this Quiz");


            }

            var sq = _mapper.Map<ShowQuiz>(quiz);
            var studentquizes =await _unitOfWork.StudentQuiz.FindAllWithIncludes<StudentQuiz>(sqq => sqq.QuizId == quiz.Id);
            var rank = studentquizes
                  .Count(otherSq => (otherSq.StudentMark + otherSq.StudentBounce) > (studentQuiz.StudentMark + studentQuiz.StudentBounce)) + 1; // Compute rank

            sq.questionsOfQuizzes = await questionService.GetAllQuestionsOfQuiz(quiz.Id, true);
            return Ok(new
            {
                rank=rank,
                Quiz=sq,
                StudentSolve=res
            });
        }
        [Authorize(Roles = "Teacher")]
        [HttpGet("GetDescreptionOfQuiz")]
        public async Task<IActionResult> GetDes(int quizId)
        {
            var loggedInId = User.FindFirst("LoggedId")?.Value;
            if (string.IsNullOrEmpty(loggedInId))
            {
                return Unauthorized("User not found");
            }
            var quiz = (await _unitOfWork.Quiz.GetByIdAsync(quizId));
            if (quiz == null)
            {
                return NotFound("Quiz not found");
            }

            if (quiz.TeacherId != int.Parse(loggedInId))
            {
                return BadRequest("You do not have permission to get this Quiz");
            }
            if (quizId == 0) return BadRequest($"There is no Quiz With ID: {quizId}");
            var res =await QuizService.GetQuizDescreption(quizId);
            if (res == null) return BadRequest();

            return Ok(res);

        }
        [Authorize(Roles = "Teacher")]
        [HttpGet("GetAllStatsOfQuizId")]
        public async Task<IActionResult> GetStats(int quizId)
        {
            var loggedInId = User.FindFirst("LoggedId")?.Value;
            if (string.IsNullOrEmpty(loggedInId))
            {
                return Unauthorized("User not found");
            }
            var quiz = (await _unitOfWork.Quiz.GetByIdAsync(quizId));
            if (quiz == null)
            {
                return NotFound("Quiz not found");
            }

            if (quiz.TeacherId != int.Parse(loggedInId))
            {
                return BadRequest("You do not have permission to get this Quiz");
            }
            var res=await _unitOfWork.Question.GetStatsOfQuiz(quizId);
            if (res == null) return BadRequest();
            return Ok(res);
        }
        [Authorize(Roles= "Student")]
        [HttpGet("GetQuizesStatusOfStudentId")]
        public async Task<IActionResult> GetQuizesStatus(int studentId)
        {
            var loggedInId = User.FindFirst("LoggedId")?.Value;
             if (studentId!=int.Parse(loggedInId))
                {
                    return BadRequest("You do not have permission to get this Statues");
                }

            if (await _unitOfWork.Student.GetByIdAsync(studentId) == null)
                return BadRequest($"There Is No Student With That Id {studentId}");
            var result = await _unitOfWork.Quiz.GetQuizzesStatusByStudentId(studentId);
            if (result == null) return BadRequest();
            return Ok(result);
        }
        [Authorize(Roles = "Teacher")]
        [HttpPost("AddOnlineQuiz")]
        [Consumes("multipart/form-data")] // Explicitly define content type
        public async Task<IActionResult> CreateOn([FromForm] CreateOnlineQuizDTO model)
        {

            if (ModelState.IsValid)
            {
                var loggedInId = User.FindFirst("LoggedId")?.Value;

                var groups = (await _unitOfWork.Group.GetAllAsync()).Where(g => model.GroupsIds.Contains(g.Id));
                if (groups.Any(g => g.TeacherId != model.TeacherId)||model.TeacherId!=int.Parse(loggedInId))
                    return BadRequest("You Do not Have Permission");
                try
                {
                  var response= await QuizService.CreateOnlineQuiz(model);
                   return Ok(response);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            else
            {
                return BadRequest(ModelState);
            }
        }
        [Authorize(Roles = "Student")]
        [HttpPost("AddStudentSolution")]
        public async Task<IActionResult> Submit(StudentSolutionDTO model)
        {
            if (ModelState.IsValid)
            {
                var loggedInId = User.FindFirst("LoggedId")?.Value;
                var groupofstudent = (await _unitOfWork.Student.GetByIdAsync(int.Parse(loggedInId))).GroupId;
                var groupIdsOfQuiz = (await _unitOfWork.GroupQuiz.GetAllAsync()).Where(gq => gq.QuizId == model.QuizId).Select(gq => gq.GroupId).ToList();
                if (!groupIdsOfQuiz.Contains(groupofstudent)||model.StudentId!= int.Parse(loggedInId))
                {
                    return BadRequest("You do not have permission to Solve this Quiz");
                }
                try
                {
                    int totalmark = 0;
                    int bouncemark = 0;
                    var quiz = await _unitOfWork.Quiz.GetByIdAsync(model.QuizId);
                    // Define Egypt's time zone
                    TimeZoneInfo egyptTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");

                    // Get the current time in Egypt
                    DateTime datenow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, egyptTimeZone);

                    //Before Quiz Start
                    if (quiz.StartDate > datenow)
                    {
                        return BadRequest("The Quiz Has Not Started Yet");
                    }
                    var IsHeSolveQuiz = await _unitOfWork.StudentQuiz.FindTWithExpression<StudentQuiz>(sq => sq.StudentId == int.Parse(loggedInId) && sq.QuizId == quiz.Id);

                    //in The Duration Of The Quiz
                    if (quiz.StartDate <= datenow && quiz.EndDate > datenow&&IsHeSolveQuiz!=null)
                    {
                        return BadRequest("You Have Submited a Solution For That Quiz Already .. wait Until The Quiz Ended and you can Test Your Self");
                    }
                    var f=false;
                    if (quiz.StartDate <= datenow && quiz.EndDate >= datenow)
                    {
                        f=true;
                    }
                    List<StudentAnswer> answers = new List<StudentAnswer>();
                    foreach (var qf in model.questionForms)
                    {
                        var quest = await _unitOfWork.Question.GetByIdAsync(qf.QuestionId);
                        var studentanswer = await _unitOfWork.Choose.GetByIdAsync(qf.ChoiceId);
                        var validanswer = await _unitOfWork.Choose.ValidAnswer(qf.QuestionId);

                      
                            var answer = new StudentAnswer
                            {
                                ChosenOptionId = studentanswer != null ? studentanswer.Id : 0,
                                QuestionId = qf.QuestionId,
                                IsCorrect =  validanswer == studentanswer ? true : false

                            };
                           
                                answers.Add(answer);
                            

                        if (quest.Type==QuestionType.Mandatory)
                        {
                            totalmark += studentanswer == validanswer ? quest.Mark : 0;

                        }
                        else
                        {
                            bouncemark += studentanswer == validanswer ? quest.Mark : 0;

                        }
                    }
                    //if the date of Student Answer is valid
                    if (f)
                    {
                        StudentQuiz sq = await QuizService.CreateQuizStudent(quiz, model, totalmark, bouncemark);
                        foreach (var answer in answers)
                        {
                            answer.StudentQuizId = sq.Id;
                            await _unitOfWork.StudentAnswer.AddAsync(answer);
                        }
                        await _unitOfWork.CompleteAsync();
                    }
                   
                    return Ok(!f?totalmark+bouncemark:"Your Answer Submited SuccessFully!!");
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }

            }
            else
                return BadRequest(ModelState);
        }
        #region Add OffLine Quiz
        /*  [HttpPost("AddOfflineQuiz")]
          public async Task<IActionResult> CreateOff([FromForm] CreateOffLineQuizDto model)
          {
              if (ModelState.IsValid) 
              {
                  try
                  {
                      var quiz = _mapper.Map<Quiz>(model);
                      // Example: Save the questionForm/answerForm to the server or cloud storage

                      // Return the file path or a URL that can be used later
                      var QuestionUrl = $"/uploads/{model.QuestionForm.FileName}";
                      var AnswerUrl = $"/uploads/{model.AnswerForm.FileName}";

                      quiz.QuestionForm = QuestionUrl;
                      quiz.AnswerForm = AnswerUrl;

                      ShowQuiz shq = await ShowQuizBinding(quiz, model);
                      return Ok(shq);
                  }
                  catch (Exception ex)
                  {

                      return BadRequest(ex.Message);
                  }


              }
              else
                  return BadRequest(ModelState);
          }*/
        #endregion
        [Authorize(Roles = "Teacher")]
        [HttpPut("UpdateOnlineQuizBeforeStart")]
        [Consumes("multipart/form-data")] // Explicitly define content type
        public async Task<IActionResult> UpdateOn([FromForm] UpdateOnlineQuizDto model)
        {
            if (ModelState.IsValid) 
            {
                var loggedInId = User.FindFirst("LoggedId")?.Value;

                var groups = (await _unitOfWork.Group.GetAllAsync()).Where(g => model.GroupsIds.Contains(g.Id));
                if (groups.Any(g => g.TeacherId != model.TeacherId) || model.TeacherId != int.Parse(loggedInId))
                    return BadRequest("You Do not Have Permission");
                try
                {

                    var quizdto = _mapper.Map<Quiz>(model);
                    var quiz = await _unitOfWork.Quiz.GetByIdAsync(quizdto.Id);
                    // Define Egypt's time zone
                    TimeZoneInfo egyptTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");

                    // Get the current time in Egypt
                    DateTime datenow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, egyptTimeZone);
                    if (quiz.StartDate <= datenow)
                        return BadRequest("The Quiz Is Started");
                   await QuizService.deleteQuiz(quizdto.Id);
                    model.Id = 0;
                    var response = await QuizService.CreateOnlineQuiz(model);
                    return Ok(response);

                   
                }
                catch (Exception ex)
                {

                    return BadRequest(ex.Message);
                }
                
            }
            else
                return BadRequest(ModelState);
        }

        #region Update Offline Quiz
        /* [HttpPut("UpdateOfflineQuiz")]
         public async Task<IActionResult> UpdateOff([FromForm]UpdateOfflineQuizDto model)
         {
             if (ModelState.IsValid)
             {
                 try
                 {
                     var quiz = _mapper.Map<Quiz>(model);
                     _unitOfWork.Quiz.Update(quiz);
                    await _unitOfWork.CompleteAsync();
                     return Ok(quiz);

                 }
                 catch (Exception ex)
                 {

                     return BadRequest(ex.Message);
                 }

             }
             else
                 return BadRequest(ModelState);
         }
 */
        #endregion
        [Authorize(Roles = "Teacher,Admin")]
        [HttpDelete("DeleteQuiz")]
        public async Task<IActionResult> Delete(int id)
        {
            var loggedInId = User.FindFirst("LoggedId")?.Value;
            var TeacherOfQuiz = (await _unitOfWork.Quiz.GetByIdAsync(id)).TeacherId;
            if (TeacherOfQuiz != int.Parse(loggedInId))
                return BadRequest("You Do not Have Permission To Delete This Quiz");
            try
            {
               
                // delete from studentquizes
                if (id == 0) return BadRequest($"There is No Quiz With Id: {id}");
             await QuizService.deleteQuiz(id);
                return Ok();
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
           
        }

    }
}
