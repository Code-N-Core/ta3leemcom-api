using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using PlatformAPI.Core.DTOs.Quiz;
using PlatformAPI.Core.Helpers;
using PlatformAPI.Core.Models;
using PlatformAPI.Core.Services;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace PlatformAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;




        public QuizController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet("GetAllQuizsByGroupId")]
        public async Task<IActionResult> GetAllByGroup(int GroupId)
        {
            var quizs =await _unitOfWork.Quiz.GetQuizzesByGroupId(GroupId);
            if (quizs == null) return NotFound($"There is No Quizs For This Group With Id {GroupId}");
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
            return Ok(sq);
        }
        [HttpGet("GetAllResultsOfQuizId")]
        public async Task<IActionResult> GetResults(int id)
        {
            if (id == 0) return BadRequest($"There is no Quiz With ID: {id}");
            var studentQuizzes = await _unitOfWork.StudentQuiz.FindAllWithIncludes<StudentQuiz>(q => q.QuizId == id,
                Sq => Sq.Quiz
                );
            List<StudentQuizResult> quizsResults = new List<StudentQuizResult>();
            foreach (var sq in studentQuizzes)
            {
                var sqr = new StudentQuizResult
                {
                    StudentId = sq.StudentId,
                    QuizId = sq.QuizId,
                    StudentMark = sq.StudentMark,
                    IsAttend = sq.IsAttend,
                    QuizMark = sq.Quiz.Mark,
                };
                quizsResults.Add(sqr);
            }
            return Ok(quizsResults);
        }
        [HttpPost("AddOnlineQuiz")]
        public async Task<IActionResult> CreateOn(CreateOnlineQuizDTO model)
        {
            if (ModelState.IsValid) 
            {
                try
                {
                    var quiz = _mapper.Map<Quiz>(model);

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
        }

        [HttpPost("AddOfflineQuiz")]
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
        }

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

        [HttpPut("UpdateOfflineQuiz")]
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

        [HttpPost("AddStudentSolution")]
        public async Task<IActionResult> Submit(StudentSolutionDTO model)
        {
            if (ModelState.IsValid)
            {
                int totalmark = 0;
                var quiz =await _unitOfWork.Quiz.GetByIdAsync(model.QuizId);
                foreach (var qf in model.questionForms)
                {
                    var quest = await _unitOfWork.Question.GetByIdAsync(qf.QuestionId);
                    var studentanswer = await _unitOfWork.Choose.GetByIdAsync(qf.ChoiceId);
                    var validanswer = await _unitOfWork.Choose.ValidAnswer(qf.QuestionId);

                    totalmark += studentanswer == validanswer ? quest.Mark : 0;
                }
                StudentQuiz sq=new StudentQuiz 
                {
                    QuizId = model.QuizId,
                    StudentId=model.StudentId,
                    IsAttend= model.SubmitAnswersDate <= quiz.EndDate?true:false,
                    StudentMark=totalmark,
                };
              await  _unitOfWork.StudentQuiz.AddAsync(sq);
                await _unitOfWork.CompleteAsync();
                StudentQuizResult quizResult = new StudentQuizResult
                {
                    StudentId = sq.StudentId,
                    QuizId=sq.QuizId,
                    StudentMark=sq.StudentMark,
                    IsAttend=sq.IsAttend,
                    QuizMark=quiz.Mark,
                };
               return Ok(quizResult);
            }
            else
                return BadRequest(ModelState);
        }

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

        private async Task<ShowQuiz> ShowQuizBinding(Quiz quiz,CreateQuizDTO model)
        {
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
            return shq;
        }
    }
}
