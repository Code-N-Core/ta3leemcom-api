using Microsoft.AspNetCore.Authorization;
using PlatformAPI.Core.DTOs.Choose;
using PlatformAPI.Core.DTOs.Questions;

namespace PlatformAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QeustionsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly AttachmentService _attachmentService;
        private readonly QuestionService _questionService;
        private readonly IImageService _imageService;
        private const string ImagesFolderForQuestions = "uploads/Questions";
        private const string ImagesFolderForChioces = "uploads/Chioces";

        public QeustionsController(IUnitOfWork unitOfWork, IMapper mapper,
            AttachmentService attachmentService, QuestionService questionService, IImageService imageService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _attachmentService = attachmentService;
            _questionService = questionService;
            _imageService = imageService;
        }
        #region all questions of quizid
        /*  [HttpGet("GetAllQuestionOfQuizID")]
          public async Task<IActionResult> GetAll(int id)
          {
              var Q = await _unitOfWork.Question.FindAllWithIncludes<Question>(q => q.QuizId == id,
                  q => q.Chooses,
                  q => q.Quiz
                  );
              if (Q == null) return NotFound($"There is No Questions With Quiz id: {id}");
              List<ShowQuestionsOfQuiz> show = new List<ShowQuestionsOfQuiz>();
              foreach (var q in Q)
              {
                  var s = QuestionService.GetQuestionMap(q);
                  show.Add(s);
              }
              return Ok(show);
          }*/
        #endregion
        [Authorize]
        [HttpGet("GetQuestion")]
        public async Task<IActionResult> GetById(int id) 
        {
            var q = await _unitOfWork.Question.FindTWithIncludes<Question>(id,q=>q.Chooses,q=>q.Quiz);
            if (q == null)
                return NotFound($"There is No Question with id: {id}");
            var s=QuestionService.GetQuestionMap(q,true);
            return Ok(s);
        }
        [Authorize]
        [HttpGet("GetQuestionsResultOfStudentQuizId")]
        public async Task<IActionResult> GetResults(int StudentQuizId)
        {
            if (StudentQuizId == 0) return BadRequest($"There is No Solution! ");
            var questionresults =await _questionService.GetStudentAnswers(StudentQuizId);
            if (questionresults == null) return NotFound();
            return Ok(questionresults);
        }

        /* [HttpPost("upload-file")]
         public async Task<IActionResult> UploadFile([FromForm] File file)
         {
             if (file.file == null || file.file.Length == 0)
             {
                 return BadRequest("No file was uploaded.");
             }

             // Example: Save the file to the server or cloud storage


             // Return the file path or a URL that can be used later
             var fileUrl = $"/uploads/{file.file.FileName}";

             //return the type of the attachment
             var attachmentType = _attachmentService.GetAttachmentType(file.file.FileName);
             if (attachmentType == "unknown")
                 return BadRequest("Unsupported file type.");

             return Ok(new { Url = fileUrl,
             Type=attachmentType
             });
         }*/


        [Authorize(Roles = "Teacher")]
        [HttpDelete("DeleteQuestionAfterStarted")]
        public async Task<IActionResult> Delete( int id)
        {
            if (ModelState.IsValid)
            {
                var q = await _unitOfWork.Question.GetByIdAsync(id);
                
                if (q == null) return BadRequest($"There Is No Question With ID {id}");
                var TecherOfQ = (await _unitOfWork.Quiz.GetByIdAsync(q.QuizId)).TeacherId;
                var loggedInId = User.FindFirst("LoggedId")?.Value;
                if (string.IsNullOrEmpty(loggedInId))
                {
                    return Unauthorized("User not found");
                }
                if (TecherOfQ != int.Parse(loggedInId))
                {
                    return BadRequest("You Dont Have Premission To Delete This Question");
                }
                try
                {
                    await _questionService.ModifiyQuiz(q,true);

                   await _questionService.DeleteQuestionsWithChoises(id);

                }
                catch (Exception ex)
                {

                    return BadRequest(ex.Message);
                }
                   await _unitOfWork.CompleteAsync();
                return Ok();


            }
            else
                return NotFound(ModelState);
        }
        [Authorize(Roles = "Teacher")]
        [HttpPut("Edit-QuestionAfterStarted")]
        public async Task<IActionResult> Update([FromBody] UQDTO model)
        {
            if (ModelState.IsValid)
            {
                var existingQuestion = await _unitOfWork.Question.GetByIdAsync(model.Id);

                if (existingQuestion == null)
                    return BadRequest("Question not found");


                var TecherOfQ = (await _unitOfWork.Quiz.GetByIdAsync(existingQuestion.QuizId)).TeacherId;
                var loggedInId = User.FindFirst("LoggedId")?.Value;
                if (string.IsNullOrEmpty(loggedInId))
                {
                    return Unauthorized("User not found");
                }
                if (TecherOfQ != int.Parse(loggedInId))
                {
                    return BadRequest("You Dont Have Premission To Update This Question");
                }

                bool isUpdated = existingQuestion.IsUpdated;

                var quiz = await _unitOfWork.Quiz.GetByIdAsync(existingQuestion.QuizId);
                // Define Egypt's time zone
                TimeZoneInfo egyptTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");

                // Get the current time in Egypt
                DateTime dateNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, egyptTimeZone);


                if (!(quiz.StartDate <= dateNow))
                    return BadRequest("The Quiz Is Not Started");

                try
                {
                    // Map only updated properties
                    _mapper.Map(model, existingQuestion); // Map the updated properties into the existing entity
                    _imageService.DeleteImage(existingQuestion.attachmentPath);
                    if(model.AttachFile != null)
                    {
                       existingQuestion.attachmentPath= _imageService.SaveImage(model.AttachFile, ImagesFolderForQuestions);
                    }
                    existingQuestion.Chooses = null; // Ensure Choices are handled separately
                    var deletedchoicess =(await _unitOfWork.Question.FindTWithIncludes<Question>(model.Id,q=>q.Chooses))
                        .Chooses.Where(c=>!(model.Choices.Select(mc=>mc.Id).Contains(c.Id)));
                    foreach (var deletedChoice in deletedchoicess)
                    {
                      await _questionService.DeleteChoice(deletedChoice);
                    }
                    var x = (await _unitOfWork.Question.FindTWithIncludes<Question>(model.Id, q => q.Chooses))
                        .Chooses.Select(c => c.Id);
                    var addedChoices = model.Choices.Where(c => !x.Contains(c.Id)).Select(c=>c.Id);

                    foreach (var ch in model.Choices)
                    {

                        if (addedChoices.Contains(ch.Id))
                        {
                            var newChoice = _mapper.Map<Choose>(ch);
                            newChoice.Id = 0;
                            newChoice.QuestionId = existingQuestion.Id;
                            if (ch.AttachFile != null)
                                newChoice.attachmentPath = _imageService.SaveImage(ch.AttachFile, ImagesFolderForChioces);
                            await _unitOfWork.Choose.AddAsync(newChoice);
                        }
                        else if ( ch.Id != 0)
                        {
                            var choice = await _unitOfWork.Choose.GetByIdAsync(ch.Id);
                           choice.Content=ch.Content;
                           choice.IsCorrect=ch.IsCorrect is not null ?(bool)ch.IsCorrect:choice.IsCorrect;
                            _imageService.DeleteImage(choice.attachmentPath);
                            if (ch.AttachFile != null)
                            {
                                choice.attachmentPath = _imageService.SaveImage(ch.AttachFile, ImagesFolderForChioces);
                            }
                            _unitOfWork.Choose.Update(choice);
                        }
                        
                    }

                    // Handle file attachment if necessary (this is commented out for now in your example)

                    if (!isUpdated)
                    {
                        await _questionService.ModifiyQuiz(existingQuestion,false);
                        existingQuestion.IsUpdated = true;
                    }

                    _unitOfWork.Question.Update(existingQuestion);

                    await _unitOfWork.CompleteAsync();
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }

                return Ok();
            }

            return BadRequest();
        }
    }
}
