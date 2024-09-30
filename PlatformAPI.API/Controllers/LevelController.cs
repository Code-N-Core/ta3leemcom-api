using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlatformAPI.Core.DTOs.Groub;
using PlatformAPI.Core.DTOs.Level;
using PlatformAPI.Core.DTOs.LevelYear;

namespace PlatformAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LevelController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public LevelController(IUnitOfWork unitOfWork) 
        {
            _unitOfWork = unitOfWork;
        }
        [Authorize(Roles ="Teacher,Admin")]
        [HttpGet("GetAllLevels")]
        public async Task<IActionResult> GetAllLevels(int teacherId)
        {
            if (await _unitOfWork.Teacher.GetByIdAsync(teacherId) == null)
                return BadRequest($"No teacher with id:{teacherId}");
            var levelsDto=new List<LevelDTO>();
            var levels=await _unitOfWork.Level.GetAllAsync();
            foreach (var level in levels)
            {
                var levelYears = await _unitOfWork.LevelYear.FindAllAsync(l=>l.LevelId==level.Id);
                var levelYearsDto=new List<LevelYearDTO>();
                foreach (var levelYear in levelYears)
                {
                    var _groups = await _unitOfWork.Group.FindAllAsync(g => g.LevelYearId == levelYear.Id && g.TeacherId == teacherId);
                    var groupsDto = new List<LevelGroupDTO>();
                    foreach (var _group in _groups)
                    {
                        var groupDto = new LevelGroupDTO()
                        {
                            GroupId = _group.Id,
                            GroupName = _group.Name,
                        };
                        groupsDto.Add(groupDto);
                    }
                    var levelYearDto = new LevelYearDTO
                    {
                        LevelYearId = levelYear.Id,
                        LevelYearName = levelYear.Name,
                        LevelGroups = groupsDto
                    };
                    levelYearsDto.Add(levelYearDto);
                }
                var levelDto = new LevelDTO
                {
                    LevelId = level.Id,
                    LevelNames=level.Name,
                    LevelYears=levelYearsDto
                };
                levelsDto.Add(levelDto);
            }
            return Ok(levelsDto);
        }
    }
}
