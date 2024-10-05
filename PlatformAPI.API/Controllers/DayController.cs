using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlatformAPI.Core.DTOs.Day;

namespace PlatformAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DayController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IMapper _mapper;
        private readonly IStudentAbsenceService _studentAbsenceService;

        public DayController(IUnitOfWork unitOfWork, IMapper mapper, IStudentAbsenceService studentAbsenceService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _studentAbsenceService = studentAbsenceService;
        }
        [Authorize(Roles = "Teacher")]
        [HttpPost]
        public async Task<IActionResult> AddAsync(DayDTO model)
        {
            var loggedInId = User.FindFirst("LoggedId")?.Value;
            var groupOfMonth = ((await _unitOfWork.Month.GetAllAsync()).Where(m => m.Id == model.MonthId)).Select(m=>m.GroupId).FirstOrDefault();
            var teacherOfGroup = (await _unitOfWork.Group.GetAllAsync()).Where(g => g.Id == groupOfMonth).Select(g => g.TeacherId).FirstOrDefault();
            if (teacherOfGroup != int.Parse(loggedInId))
                return BadRequest("You Do Not Have The Premission");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (await _unitOfWork.Month.GetByIdAsync(model.MonthId) == null)
                return BadRequest($"No month with id: {model.MonthId}");
            if(await _unitOfWork.Day.FindTWithExpression<Day>(d=>d.MonthId==model.MonthId&&d.Date==model.Date)!=null)
                return BadRequest("You try to repeat day");
            var day=_mapper.Map<Day>(model);
            try
            {
                await _unitOfWork.Day.AddAsync(day);
            }
            catch(Exception ex)
            {
                return BadRequest(ex);
            }
            await _unitOfWork.CompleteAsync();
            try
            {
                await _studentAbsenceService.AddAsync(day.Id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
            var viewDayDTO=_mapper.Map<ViewDayDTO>(day);
            viewDayDTO.studentAbsences = await _studentAbsenceService.GetAllAsync(viewDayDTO.Id);
            return Ok(viewDayDTO);
        }
        [Authorize(Roles = "Teacher")]
        [HttpDelete]
        public async Task<IActionResult> DeleteAsync([FromQuery]int dayId)
        {
            var day = await _unitOfWork.Day.GetByIdAsync(dayId);
            if (day == null)
                return NotFound($"No day with id: {dayId}");


            var loggedInId = User.FindFirst("LoggedId")?.Value;
            var groupOfMonth = ((await _unitOfWork.Month.GetAllAsync()).Where(m => m.Id == day.MonthId)).Select(m => m.GroupId).FirstOrDefault();
            var teacherOfGroup = (await _unitOfWork.Group.GetAllAsync()).Where(g => g.Id == groupOfMonth).Select(g => g.TeacherId).FirstOrDefault();
            if (teacherOfGroup != int.Parse(loggedInId))
                return BadRequest("You Do Not Have The Premission");


          
            var studentsAbsence=await _unitOfWork.StudentAbsence.FindAllAsync(sa=>sa.DayId==dayId);
            if (studentsAbsence.Count() != 0)
            {
                foreach(var studentAbsence in studentsAbsence)
                {
                    try
                    {
                        await _unitOfWork.StudentAbsence.DeleteAsync(studentAbsence);
                    }
                    catch (Exception ex)
                    {
                        return BadRequest(ex);
                    }
                }
            }
            try
            {
                await _unitOfWork.Day.DeleteAsync(day);
            }
            catch(Exception ex)
            {
                return BadRequest(ex);
            }
            await _unitOfWork.CompleteAsync();
            return Ok("Day deleted successfully");
        }
    }
}