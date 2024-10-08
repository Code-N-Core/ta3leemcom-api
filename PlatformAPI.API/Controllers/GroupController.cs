﻿using Microsoft.AspNetCore.Authorization;
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
        [Authorize(Roles = "Teacher")]
        [HttpGet("GetAllGroupsOfTeacherId")]
        public async Task<IActionResult> GetAllAsync(int levelYearId) 
        {
            var loggedInId = User.FindFirst("LoggedId")?.Value;
            if (string.IsNullOrEmpty(loggedInId))
            {
                return Unauthorized("User not found");
            }
            //if id==0 return all the groups int the system

            var Groups = await _unitOfWork.Group.FindAllAsync(g => (g.TeacherId == int.Parse(loggedInId) 
            || int.Parse(loggedInId) == 0)&&g.LevelYearId==levelYearId);
            
            if(Groups is null)
                return  NotFound($"there is no Group With Teacher Id: {int.Parse(loggedInId)}");

            return  Ok(Groups);
        }
        [Authorize(Roles = "Teacher")]
        [HttpGet("GetGroup")]
        public async Task<IActionResult> GetById(int id)
        {
            var loggedInTeacherId = User.FindFirst("LoggedId")?.Value;

            if (string.IsNullOrEmpty(loggedInTeacherId))
            {
                return Unauthorized("User not found");
            }
            var g = (await _unitOfWork.Group.GetAllAsync()).Where(g =>g.Id==id).FirstOrDefault();
            if (g == null)
            {
                return NotFound("Group not found");
            }

            if (g.TeacherId != int.Parse(loggedInTeacherId))
            {
                return BadRequest("You do not have permission to get this group");
            }


            var group =await _unitOfWork.Group.FindTWithIncludes<Group>(id,
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
            return Ok(new
            {
                Id = id,
                LevelYearId = group.LevelYear.Id,
                LevelYearName = group.LevelYear.Name,
                Name = group.Name,
                TeacherId = group.TeacherId,
                Students = listStudents,
                LevelId = group.LevelYear.LevelId,
                LevelName = group.LevelYear.Level.Name
            });
        }
        [Authorize(Roles = "Teacher")]
        [HttpGet("GetResultsOfStudnetsInGruopID")]
        public async Task<IActionResult> GetResultInListOfGruops([FromQuery]List<int> groupids)
        {
            var loggedInTeacherId = User.FindFirst("LoggedId")?.Value;

            if (string.IsNullOrEmpty(loggedInTeacherId))
            {
                return Unauthorized("User not found");
            }
            var groups = (await _unitOfWork.Group.GetAllAsync()).Where(g => groupids.Contains(g.Id));
            if (groups == null)
            {
                return NotFound("Group not found");
            }

            if (groups.Any(g => g.TeacherId != int.Parse(loggedInTeacherId)))
            {
                return BadRequest("You do not have permission to get These groups");
            }



            if (groupids is null) return BadRequest($"There Groups is Null");
            var quizids = await _unitOfWork.Group.Getquizsresults(groupids);
            var res = await _studentQuizService.GetAllStudentQuizResults(quizids);
            return Ok(res.OrderByDescending(s=>s.StudentMark));

        }
        [Authorize(Roles = "Teacher")]
        [HttpPost("Add")]
        public async Task<IActionResult> AddAsync(AddGroupDTO model)
        {

            if(ModelState.IsValid)
            {
                var loggedInTeacherId = User.FindFirst("LoggedId")?.Value;

                if (string.IsNullOrEmpty(loggedInTeacherId))
                {
                    return Unauthorized("User not found");
                }

                if (model.TeacherId!= int.Parse(loggedInTeacherId))
                {
                    return BadRequest("You do not have permission");
                }


                if (await _unitOfWork.LevelYear.GetByIdAsync(model.LevelYearId) == null)
                    return BadRequest($"there is no LevelYear with LevelYearId {model.LevelYearId}");
                if(await _unitOfWork.Teacher.GetByIdAsync(model.TeacherId) == null)
                    return BadRequest($"there is no Teacher with TeacherId {model.LevelYearId}");

                var group = _mapper.Map<PlatformAPI.Core.Models.Group>(model);
                try
                {
                    await _unitOfWork.Group.AddAsync(group);
                   await _unitOfWork.CompleteAsync();
                    var groupDto=_mapper.Map<GroupDTO>(group);
                    return Ok(new
                    {
                        Id = groupDto.Id,
                        Name = groupDto.Name,
                        LevelYearId = groupDto.LevelYearId,
                        TeacherId = groupDto.TeacherId,
                        LevelYearName = _unitOfWork.LevelYear.GetByIdAsync(group.LevelYearId).Result.Name,
                        LevelId = _unitOfWork.LevelYear.GetByIdAsync(group.LevelYearId).Result.LevelId,
                        LevelName =_unitOfWork.Level.GetByIdAsync( _unitOfWork.LevelYear.GetByIdAsync(group.LevelYearId).Result.LevelId).Result.Name
                    });
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
        [Authorize(Roles = "Teacher")]
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(int id) 
        {
            var loggedInId = User.FindFirst("LoggedId")?.Value;
            var TeacherOfGroup = (await _unitOfWork.Group.GetByIdAsync(id)).TeacherId;
            if (TeacherOfGroup != int.Parse(loggedInId))
                return BadRequest("You Do not Have Permission To Delete This Group");

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
        [Authorize(Roles = "Teacher")]
        [HttpPut("Edit")]
        public async Task<IActionResult> Update([FromForm] GroupDTO group)
        {
            if (ModelState.IsValid)
            {
                var loggedInId = User.FindFirst("LoggedId")?.Value;
                var TeacherOfGroup = (await _unitOfWork.Group.GetByIdAsync(group.Id)).TeacherId;
                if (TeacherOfGroup != int.Parse(loggedInId) || group.TeacherId!=TeacherOfGroup)
                    return BadRequest("You Do not Have Permission To Delete This Group");

                var g = await _unitOfWork.Group.GetByIdAsync(group.Id);
                if (g == null) return NotFound($"No Group was found with ID {group.Id}");
                try
                {
                    g.Name = group.Name;
                    g.LevelYearId =(int)group.LevelYearId;
                    g = _unitOfWork.Group.Update(g);
                   await _unitOfWork.CompleteAsync();
                    return Ok(
                        new
                        {
                            Id = g.Id,
                            Name = g.Name,
                            LevelYearId = g.LevelYearId,
                            TeacherId = g.TeacherId,
                            LevelYearName = _unitOfWork.LevelYear.GetByIdAsync(g.LevelYearId).Result.Name,
                            LevelId = _unitOfWork.LevelYear.GetByIdAsync(g.LevelYearId).Result.LevelId,
                            LevelName = _unitOfWork.Level.GetByIdAsync(_unitOfWork.LevelYear.GetByIdAsync(g.LevelYearId).Result.LevelId).Result.Name
                        }
                        );
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