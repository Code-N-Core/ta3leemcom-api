using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using PlatformAPI.Core.DTOs.Quiz;
using PlatformAPI.Core.Helpers;
using PlatformAPI.Core.Models;
using PlatformAPI.Core.Services;
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

        [HttpGet("GetAllQuizsByGroupsIds")]
        public async Task<IActionResult> GetAllByGroup([FromQuery]List<int> GroupsIds)
        {
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
        [HttpGet("GetQuizById")]
        public async Task<IActionResult>GetById(int QuizId)
        {
            var quizs =await _unitOfWork.Quiz.FindTWithIncludes<Quiz>(QuizId,q=>q.GroupsQuizzes);
            if (quizs == null) return NotFound($"There is No Quizs With Id {QuizId}");
            var sq=_mapper.Map<ShowQuiz>(quizs);
            sq.GroupsIds=new List<int> ();
            foreach (var gq in quizs.GroupsQuizzes)
            {
                sq.GroupsIds.Add(gq.GroupId);
            }
            sq.questionsOfQuizzes =await questionService.GetAllQuestionsOfQuiz(QuizId);
            return Ok(sq);
        }
        [HttpGet("GetAllResultsOfQuizId")]
        public async Task<IActionResult> GetResults(int quizId)
        {
            if (quizId == 0) return BadRequest($"There is no Quiz With ID: {quizId}");
            var studentQuizzes = await _unitOfWork.StudentQuiz
                .FindAllWithIncludes<StudentQuiz>(q => q.QuizId == quizId && q.IsAttend,
                    Sq => Sq.Quiz);
            var res=await QuizService.GetAllStudentQuizResults(studentQuizzes);
            if (res == null) return BadRequest();

            return Ok(res);
        }

        [HttpGet("GetDescreptionOfQuiz")]
        public async Task<IActionResult> GetDes(int quizId)
        {
            if (quizId == 0) return BadRequest($"There is no Quiz With ID: {quizId}");
            var res =await QuizService.GetQuizDescreption(quizId);
            if (res == null) return BadRequest();

            return Ok(res);

        }
        [HttpGet("GetAllStatsOfQuizId")]
        public async Task<IActionResult> GetStats(int quizid)
        {
            var res=await _unitOfWork.Question.GetStatsOfQuiz(quizid);
            if (res == null) return BadRequest();
            return Ok(res);
        }

        [HttpGet("GetQuizesStatusOfStudentId")]
        public async Task<IActionResult> GetQuizesStatus(int studentId)
        {
            var result= await _unitOfWork.Quiz.GetQuizzesStatusByStudentId(studentId);
            if (result == null) return BadRequest();
            return Ok(result);
        }

        [HttpPost("AddOnlineQuiz")]
        public async Task<IActionResult> CreateOn(CreateOnlineQuizDTO model)
        {
            if (ModelState.IsValid) 
            {
                try
                {
                    var quiz = _mapper.Map<Quiz>(model);
                    quiz.StartDate = QuizService.GetDateTimeFromTimeStart(model.timeStart, model.StartDate);
                    quiz.Duration = new TimeSpan(model.timeDuration.Days, model.timeDuration.Hours, model.timeDuration.Minute, 0);
                    //addQuiz
                    await _unitOfWork.Quiz.AddAsync(quiz);
                    await _unitOfWork.CompleteAsync();
                    ShowQuiz shq = _mapper.Map<ShowQuiz>(quiz);
                    shq.GroupsIds = new List<int>();
                    foreach (var group in model.GroupsIds)
                    {
                        GroupQuiz gq = new GroupQuiz
                        {
                            GroupId = group,
                            QuizId = quiz.Id,
                        };
                        await _unitOfWork.GroupQuiz.AddAsync(gq);
                        shq.GroupsIds.Add(gq.GroupId);
                    }
                    await _unitOfWork.CompleteAsync();
                    //addQuestion
                    foreach (var q in model.Questions)
                    {
                        q.QuizId=quiz.Id;
                       var quest=await questionService.CreateQuestion(q);
                        //addChoice
                        foreach (var c in q.Choices)
                        {
                            c.QuestionId=quest.Id;
                           await chooseService.CreateChoose(c);
                        }
                    }

                    return Ok(shq);
                }
                catch (Exception ex)
                {

                    return BadRequest(ex.Message);
                }
            }
            else
                return BadRequest(ModelState);
        }
        [HttpPost("AddStudentSolution")]
        public async Task<IActionResult> Submit(StudentSolutionDTO model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int totalmark = 0;
                    int bouncemark = 0;
                    var quiz = await _unitOfWork.Quiz.GetByIdAsync(model.QuizId);
                    List<StudentAnswer> answers = new List<StudentAnswer>();
                    foreach (var qf in model.questionForms)
                    {
                        var quest = await _unitOfWork.Question.GetByIdAsync(qf.QuestionId);
                        var studentanswer = await _unitOfWork.Choose.GetByIdAsync(qf.ChoiceId);
                        var validanswer = await _unitOfWork.Choose.ValidAnswer(qf.QuestionId);

                        if (studentanswer != null)
                        {
                            var answer = new StudentAnswer
                            {
                                ChosenOptionId = studentanswer != null ? studentanswer.Id : 0,
                                QuestionId = qf.QuestionId,
                                IsCorrect = studentanswer == validanswer ? true : false

                            };
                            answers.Add(answer);
                        }

                        if (quest.Type==QuestionType.Mandatory)
                        {
                            totalmark += studentanswer == validanswer ? quest.Mark : 0;

                        }
                        else
                        {
                            bouncemark += studentanswer == validanswer ? quest.Mark : 0;

                        }
                    }
                    StudentQuiz sq = await QuizService.CreateQuizStudent(quiz, model, totalmark, bouncemark);
                    foreach (var answer in answers)
                    {
                        answer.StudentQuizId = sq.Id;
                        await _unitOfWork.StudentAnswer.AddAsync(answer);
                    }
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

        [HttpPut("UpdateOnlineQuiz")]
        public async Task<IActionResult> UpdateOn(UpdateOnlineQuizDto model)
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
        
        [HttpDelete("DeleteQuiz")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // delete from studentquizes
                if (id == 0) return BadRequest($"There is No Quiz With Id: {id}");
                var quiz = await _unitOfWork.Quiz.GetByIdAsync(id);
                await _unitOfWork.Quiz.DeleteAsync(quiz);
               await _unitOfWork.CompleteAsync();
                return Ok();
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
           
        }

    }
}
