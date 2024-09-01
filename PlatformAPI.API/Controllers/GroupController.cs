using PlatformAPI.Core.DTOs.Groub;
using PlatformAPI.Core.DTOs.Quiz;
using PlatformAPI.Core.DTOs.Student;
using Group = PlatformAPI.Core.Models.Group;
namespace PlatformAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly QuizService _studentQuizService;
        public GroupController(IUnitOfWork unitOfWork, IMapper mapper, QuizService studentQuizService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _studentQuizService = studentQuizService;
        }
        [HttpGet("GetAllGroupsOfTeacherId")]
        public async Task<IActionResult> GetAllAsync(int teacherId,int levelYearId) 
        {
            //if id==0 return all the groups int the system
           
               var Groups = await _unitOfWork.Group.FindAllAsync(g => (g.TeacherId == teacherId || teacherId == 0)&&g.LevelYearId==levelYearId);
            
            if(Groups is null)
                return  NotFound($"there is no Group With Teacher Id: {teacherId}");

            return  Ok(Groups);
        }

        [HttpGet("GetGroup")]
        public async Task<IActionResult> GetById(int id)
        {
          
           var group=await _unitOfWork.Group.FindTWithIncludes<Group>(id,
               g=>g.Teacher,
               g=>g.Students,
               g=>g.GroupQuizzes,
               g=>g.LevelYear,
               g=>g.LevelYear.Level,
               g=>g.Months
            );
            if(group is null)
                return NotFound($"there is no Group With Id {id}");
            var listStudents=new List<StudentDTO>();
            foreach(var student in group.Students)
            {
                var s=_mapper.Map<StudentDTO>(student);
                s.LevelYearName=group.LevelYear.Name;
               /* var Level=await _unitOfWork.Level.GetByIdAsync(group.LevelYear.LevelId);
                s.LevelName=Level.Name;*/
               s.LevelName=group.LevelYear.Level.Name;
                listStudents.Add(s);
            }
            var data = new GroupDTO()
            {
                Id = id,
                LevelYearId = group.LevelYear.LevelId,
                Name = group.Name,
                TeacherId = group.TeacherId,
                Students = listStudents
            };
            return Ok(data);
        }
        [HttpGet("GetResultsOfStudnetsInGruopID")]
        public async Task<IActionResult> GetResultInListOfGruops([FromQuery]List<int> groupids)
        {
            if (groupids is null) return BadRequest($"There Groups is Null");
            var quizids = await _unitOfWork.Group.Getquizsresults(groupids);
            var res = await _studentQuizService.GetAllStudentQuizResults(quizids);
            return Ok(res.OrderByDescending(s=>s.StudentMark));

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
                    await _unitOfWork.CompleteAsync();
                    var groupDto =_mapper.Map<GroupDTO>(group);
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
        /*
         * 
         * there are more entitese need to delete
         * */

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(int id) 
        {
            if (id == 0) return BadRequest($"there is no id with {id}");
           
            var group = await _unitOfWork.Group.GetByIdAsync(id);

            if (group == null) return NotFound();

            try
            {
                var students = await _unitOfWork.Student.GetAllAsync();
                foreach (var student in students)
                {
                    if (student.GroupId == id)
                    {
                        await _unitOfWork.Student.DeleteAsync(student);
                    }
                }

                await _unitOfWork.Group.DeleteAsync(group);
               await _unitOfWork.CompleteAsync();
                return Ok();
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
           
        }
        [HttpPut("Edit")]
        public async Task<IActionResult> Update([FromForm] GroupDTO group)
        {
            if (ModelState.IsValid)
            {
                var g = await _unitOfWork.Group.GetByIdAsync(group.Id);
                if (g == null) return NotFound($"No Group was found with ID {group.Id}");
                try
                {
                    g.Name = group.Name;
                    g.LevelYearId =(int)group.LevelYearId;
                    g = _unitOfWork.Group.Update(g);
                   await _unitOfWork.CompleteAsync();
                    return Ok(g);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
               
            }
            return BadRequest(ModelState);
        }

      
    }
}