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

            var quizs =await _unitOfWork.Quiz.FindTWithIncludes<Quiz>(QuizId,q=>q.GroupsQuizzes);
            if (quizs == null) return NotFound($"There is No Quizs With Id {QuizId}");
            var sq=_mapper.Map<ShowQuiz>(quizs);
            sq.GroupsIds=new List<int> ();
            foreach (var gq in quizs.GroupsQuizzes)
            {
                sq.GroupsIds.Add(gq.GroupId);
            }
            sq.questionsOfQuizzes =await questionService.GetAllQuestionsOfQuiz(QuizId,f);
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
                .FindAllWithIncludes<StudentQuiz>(q => q.QuizId == quizId && !q.IsAttend,
                    Sq => Sq.Quiz);
            var res=await QuizService.GetAllStudentQuizResults(studentQuizzes);
            if (res == null) return BadRequest();

            return Ok(res);
        }

        [Authorize(Roles="Student")]
        [HttpGet("GetStudentSolutionByStudentQuizId")]
        public async Task<IActionResult> GetResultOfStudentQuizId(int studentQuizId)
        {
            var studentQuizzes = await _unitOfWork.StudentQuiz
                .FindAllWithIncludes<StudentQuiz>(q => q.Id == studentQuizId,
                    Sq => Sq.Quiz);

            var studentId = studentQuizzes.Select(sq => sq.StudentId).FirstOrDefault();

            var loggedInId = User.FindFirst("LoggedId")?.Value;
            if (string.IsNullOrEmpty(loggedInId))
            {
                return Unauthorized("User not found");
            }
            if (studentId != int.Parse(loggedInId))
                return BadRequest("You Do Not Have Premission");

            var res =( await QuizService.GetAllStudentQuizResults(studentQuizzes)).FirstOrDefault();
            if (res == null) return BadRequest();

            var quizs = await _unitOfWork.Quiz.FindTWithIncludes<Quiz>(res.QuizId, q => q.GroupsQuizzes);
            if (quizs == null) return NotFound($"There is No Quizs With Id {res.QuizId}");
            var sq = _mapper.Map<ShowQuiz>(quizs);
            sq.GroupsIds = new List<int>();
            foreach (var gq in quizs.GroupsQuizzes)
            {
                sq.GroupsIds.Add(gq.GroupId);
            }
            sq.questionsOfQuizzes = await questionService.GetAllQuestionsOfQuiz(quizs.Id, true);
            return Ok(new
            {
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
        public async Task<IActionResult> CreateOn(CreateOnlineQuizDTO model)
        {

            if (ModelState.IsValid)
            {
                var loggedInId = User.FindFirst("LoggedId")?.Value;

                var groups = (await _unitOfWork.Group.GetAllAsync()).Where(g => model.GroupsIds.Contains(g.Id));
                if (groups.Any(g => g.TeacherId != model.TeacherId)||model.TeacherId!=int.Parse(loggedInId))
                    return BadRequest("You Do not Have Permission");
                try
                {
                    // Map the quiz DTO to the quiz entity
                    var quiz = _mapper.Map<Quiz>(model);

                    try
                    {
                        // Assign StartDate and Duration
                        quiz.StartDate = QuizService.GetDateTimeFromTimeStart(model.timeStart, model.StartDate);
                        quiz.Duration = new TimeSpan(model.timeDuration.Days, model.timeDuration.Hours, model.timeDuration.Minute, 0);

                        // Add the quiz to the repository
                        await _unitOfWork.Quiz.AddAsync(quiz);
                        await _unitOfWork.CompleteAsync(); // Commit after adding quiz
                    }
                    catch (Exception ex)
                    {
                        return BadRequest(ex.Message);
                    }

                    // Create GroupQuiz entries
                    var shq = _mapper.Map<ShowQuiz>(quiz);
                    shq.GroupsIds = new List<int>();

                    try
                    {
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
                    }
                    catch (Exception ex)
                    {
                        return BadRequest(ex.Message);
                    }

                    // Add questions and choices
                    try
                    {
                        foreach (var q in model.Questions)
                        {
                            q.QuizId = quiz.Id;
                            var lcho = new List<ChooseDTO>();
                     
                                foreach (var c in q.Chooses)
                                {
                                    var choice = new ChooseDTO 
                                    {
                                        Content=c.Content,

                                    };
                                    lcho.Add(choice);

                                }
                            
                             await questionService.CreateQuestion(q,lcho); // Add question

                        }

                    }
                    catch (Exception ex)
                    {
                        return BadRequest(ex.Message);
                    }

                    // Retrieve and map all questions
                    shq.questionsOfQuizzes = new List<ShowQuestionsOfQuiz>(await questionService.GetAllQuestionsOfQuiz(quiz.Id,true));

                    return Ok(shq);
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
                    var datenow=DateTime.Now;
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
                   
                    return Ok(totalmark+bouncemark);
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
        public async Task<IActionResult> UpdateOn(UpdateOnlineQuizDto model)
        {
            if (ModelState.IsValid) 
            {
                var loggedInId = User.FindFirst("LoggedId")?.Value;

                var groups = (await _unitOfWork.Group.GetAllAsync()).Where(g => model.GroupsIds.Contains(g.Id));
                if (groups.Any(g => g.TeacherId != model.TeacherId) || model.TeacherId != int.Parse(loggedInId))
                    return BadRequest("You Do not Have Permission");
                try
                {

                    var quiz = _mapper.Map<Quiz>(model);
                    var datenow = DateTime.Now;
                    if (quiz.StartDate <= datenow)
                        return BadRequest("The Quiz Is Started");
                   await QuizService.deleteQuiz(quiz.Id);
                    quiz.Id = 0;
                   await _unitOfWork.Quiz.AddAsync(quiz);
                    await _unitOfWork.CompleteAsync();
                    return Ok();

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
