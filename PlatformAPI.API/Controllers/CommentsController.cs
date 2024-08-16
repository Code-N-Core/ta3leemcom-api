using Microsoft.Extensions.Options;
using PlatformAPI.Core.Helpers;

namespace PlatformAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly IMailingService _mailingService;
        private readonly MailSettings _mailSettings;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CommentsController(IMailingService mailingService, IOptions<MailSettings> mailSettings, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mailingService = mailingService;
            _mailSettings = mailSettings.Value;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        [HttpPost("Contact")]
        public async Task<IActionResult> Contact(ContactDTO model)
        {
            if(ModelState.IsValid)
            {
                if (EmailValidatorService.IsValidEmailProvider(model.Email))
                {
                    try
                    {
                        model.Message = $"From: {model.Email}<br>Name: {model.Name}<br>{model.Message}";
                        await _mailingService.SendEmailAsync(_mailSettings.Email, model.Subject, model.Message);
                        return Ok(model);
                    }
                    catch (Exception ex)
                    {
                        return BadRequest(ex);
                    }
                }
                else return BadRequest("Check email provider");
            }
            else return BadRequest(ModelState);
        }
        [HttpPost("AddFeedback")]
        public async Task<IActionResult> AddFeedbackAsync(AddFeedbackDTO model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var feedback = _mapper.Map<Feedback>(model);
                    await _unitOfWork.Feedback.AddAsync(feedback);
                    _unitOfWork.Complete();
                    return Ok(feedback);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex);
                }
            }
            else return BadRequest(ModelState);
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteFeedbackAsync(int id)
        {
            var feedback = await _unitOfWork.Feedback.GetByIdAsync(id);
            if (feedback == null)
                return BadRequest($"no feedback with id {id}");
            await _unitOfWork.Feedback.DeleteAsync(feedback);
            _unitOfWork.Complete();
            return Ok(feedback);
        }
    }
}