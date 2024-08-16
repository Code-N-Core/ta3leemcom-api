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

        public QeustionsController(IUnitOfWork unitOfWork, IMapper mapper, AttachmentService attachmentService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _attachmentService = attachmentService;
        }
        //return all questions of quiz with (id)
        [HttpGet("GetAllQuestionOfQuizID")]
        public async Task<IActionResult> GetAll(int id) 
        {
            var Q = await _unitOfWork.Question.FindAllWithIncludes<Question>(q => q.QuizId == id,
                q=>q.Chooses,
                q=>q.Quiz
                );
            if (Q == null) return NotFound($"There is No Questions With Quiz id: {id}");
            List<ShowQuestionsOfQuiz> show= new List<ShowQuestionsOfQuiz>();
            foreach (var q in Q)
            {
                var s = QuestionServ.GetQuestionMap(q);
                show.Add(s);
            }
            return Ok(show);
        }
        [HttpGet("GetQuestion")]
        public async Task<IActionResult> GetById(int id) 
        {
            var q = await _unitOfWork.Question.FindTWithIncludes<Question>(id,q=>q.Chooses,q=>q.Quiz);
            if (q == null)
                return NotFound($"There is No Question with id: {id}");
            var s=QuestionServ.GetQuestionMap(q);
            return Ok(s);
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

        [HttpPost("Add")]
        public async Task<IActionResult> Create([FromForm] QDTO q)
        {
            if (ModelState.IsValid)
            {
               
                var question=_mapper.Map<Question>(q);
                  
                question.Type = q.Type == "اجباري" ? QuestionType.Mandatory : QuestionType.Optional;

                if(q.AttachFile != null)
                {
                    // Example: Save the file to the server or cloud storage


                    // Return the file path or a URL that can be used later
                    var fileUrl = $"/uploads/{q.AttachFile.FileName}";

                    //return the type of the attachment
                    var attachmentType = _attachmentService.GetAttachmentType(q.AttachFile.FileName);
                    if (attachmentType == "unknown")
                        return BadRequest("Unsupported file type.");

                    question.attachmentPath = fileUrl;
                    question.attachmentType = attachmentType;
                    
                }
               await _unitOfWork.Question.AddAsync(question);
                 _unitOfWork.Complete();
                

                return Ok(question);

            }
            else
                return BadRequest(ModelState);
        }

        [HttpDelete("DeleteQuestion")]
        public async Task<IActionResult> Delete( int id)
        {
            if (ModelState.IsValid)
            {
                var q = await _unitOfWork.Question.GetByIdAsync(id);
                if (q == null) return BadRequest($"There Is No Question With ID {id}");
                try
                {
                    var choices = await _unitOfWork.Choose.FindAllAsync(c => c.QuestionId == id);
                    foreach (var choice in choices)
                        await _unitOfWork.Choose.DeleteAsync(choice);
                    await _unitOfWork.Question.DeleteAsync(q);
                    _unitOfWork.Complete();
                    return Ok();

                }
                catch (Exception ex)
                {

                    return BadRequest(ex.Message);
                }
                
            }
            else
                return NotFound(ModelState);
        }

        [HttpPut("Edit-Question")]
        public async Task<IActionResult> Update([FromForm] UQDTO model)
        {
            if (ModelState.IsValid)
            {
                var q = _mapper.Map<Question>(model);
                if (q == null) return BadRequest();

                if (model.AttachFile != null)
                {
                    var attachmentType = _attachmentService.GetAttachmentType(model.AttachFile.FileName);
                    if (attachmentType == "unknown")
                        return BadRequest("Unsupported file type.");

                    // Return the file path or a URL that can be used later
                    var fileUrl = $"/uploads/{model.AttachFile.FileName}";
                    
                    //if its upload new file 
                    if (q.attachmentPath != null && fileUrl!=q.attachmentPath)
                    {
                        //Delete the old url from the server
                        //save the new fileurl in the sever
                    }
                    // 
                    q.attachmentPath = fileUrl;
                    q.attachmentType = attachmentType;
                }
                _unitOfWork.Question.Update(q);
                _unitOfWork.Complete();
                return Ok(q);

            }
            else
                return BadRequest();
        }
    }
}
