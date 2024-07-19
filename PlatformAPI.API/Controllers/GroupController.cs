namespace PlatformAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public GroupController(IUnitOfWork unitOfWork,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpPost("Add")]
        public async Task<IActionResult> AddAsync(AddGroupDTO model)
        {
            if(ModelState.IsValid)
            {
                if (await _unitOfWork.LevelYear.GetByIdAsync(model.LevelYearId) == null)
                    return BadRequest($"there is no LevelYear with LevelYearId {model.LevelYearId}");
                if(await _unitOfWork.Teacher.GetByIdAsync(model.TeacherId) == null)
                    return BadRequest($"there is no Teacher with TeacherId {model.LevelYearId}");

                var group = _mapper.Map<PlatformAPI.Core.Models.Group>(model);
                try
                {
                    await _unitOfWork.Group.AddAsync(group);
                    _unitOfWork.Complete();
                    var groupDto=_mapper.Map<GroupDTO>(group);
                    return Ok(groupDto);
                }
                catch(Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            else
                return BadRequest(ModelState);
        }
    }
}