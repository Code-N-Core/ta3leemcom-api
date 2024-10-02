using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PlatformAPI.Core.DTOs.Choose;

namespace PlatformAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChooseController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;


        public ChooseController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        //return all chooses of question with (id) or return all chooses
        [HttpGet("GetAllChoosesofQuestionWithId")]
        public async Task<IActionResult> GetAll(int id)
        {
            var Chooses = await _unitOfWork.Choose.FindAllAsync(c => c.QuestionId == id||id==0);
            if (Chooses == null)
                return NotFound("There is No Chooses");
            return Ok(Chooses);

        }
        //return specific choose
        [HttpGet("GetChoiceById")]
        public async Task<IActionResult> GetById(int id) 
        {
            var choose=await _unitOfWork.Choose.GetByIdAsync(id);
            if (choose == null)
                return NotFound($"There is No Choose with id: {id}");
            return Ok(choose);
        }

       /* [HttpPost("CreateChose")]
        public async Task<IActionResult> Create(ChooseDTO model)
        {
            if (ModelState.IsValid) 
            {
                var choice = _mapper.Map<Choose>(model);
              await  _unitOfWork.Choose.AddAsync(choice);
               await _unitOfWork.CompleteAsync();
                return Ok(choice);
            }
            return BadRequest();
        }*/

        [HttpDelete]
        public async Task<IActionResult>Delete(int id)
        {
            var choose = await _unitOfWork.Choose.GetByIdAsync(id);
            if (choose == null)
                return NotFound();
           
                await _unitOfWork.Choose.DeleteAsync(choose);
           await _unitOfWork.CompleteAsync();
            return Ok();
        }

        [HttpPut("Edit-Choices")]
        public async Task<IActionResult> Update([FromBody] List<ChooseDTO> chooses)
        {
            if (ModelState.IsValid)
            {
                int f = 0;
                foreach (var choice in chooses)
                {
                    if ((bool)choice.IsCorrect)
                    {
                        f=choice.Id;
                    }
                    var c = _mapper.Map<Choose>(choice);
                    try
                    {
                        _unitOfWork.Choose.Update(c);

                    }
                    catch (Exception ex)
                    {

                       return BadRequest(ex.Message);
                    }
                }
                if(f == 0)
                    return BadRequest("يجب عليك اختيار اجابه صحيحه");
               await _unitOfWork.CompleteAsync();
                return Ok();

            }
            return BadRequest(ModelState);
        }

    }
}
