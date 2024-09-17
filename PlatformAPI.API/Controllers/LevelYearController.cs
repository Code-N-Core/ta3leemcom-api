using Microsoft.AspNetCore.Authorization;

namespace PlatformAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LevelYearController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public LevelYearController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllLevelsYearsAsync()
        {
            var levelsYears=await _unitOfWork.LevelYear.GetAllAsync();
            if(levelsYears == null)
                return NotFound();
            return Ok(levelsYears);
        }
    }
}
