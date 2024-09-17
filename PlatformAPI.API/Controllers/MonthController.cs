using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlatformAPI.Core.DTOs.Day;
using PlatformAPI.Core.DTOs.Month;
using PlatformAPI.Core.DTOs.StudentAbsence;
using PlatformAPI.Core.DTOs.StudentMonth;
namespace PlatformAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MonthController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IDayServices _dayServices;
        private readonly IStudentMonthService _studentMonthService;
        private readonly IStudentAbsenceService _studentAbsenceService;
        public MonthController(IUnitOfWork unitOfWork, IMapper mapper,IDayServices dayServices,IStudentMonthService studentMonthService,IStudentAbsenceService studentAbsenceService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _dayServices = dayServices;
            _studentMonthService = studentMonthService;
            _studentAbsenceService = studentAbsenceService;
        }
        [Authorize(Roles="Teacher")]
        [HttpGet("GetLastMonthAdded")]
        public async Task<IActionResult> GetAsync(int groupId)
        {
            if (await _unitOfWork.Group.GetByIdAsync(groupId) == null)
                return NotFound($"No group with id: {groupId}");
            int lastMonthId = _unitOfWork.Month.FindAllAsync(m => m.GroupId == groupId).Result.OrderByDescending(m => m.Id).FirstOrDefault().Id;
            var viewDaysDto = await _dayServices.GetAllAsync(lastMonthId);
            foreach(var day in viewDaysDto)
            {
                day.studentAbsences = await _studentAbsenceService.GetAllAsync(day.Id);
            }
            var monthStudents=await _studentMonthService.GetAllAsync(lastMonthId);
            var monthData = new ViewMonthDTO
            {
                Id = lastMonthId,
                GroupId = groupId,
                Name = _unitOfWork.Month.FindAllAsync(m => m.GroupId == groupId).Result.OrderByDescending(m => m.Id).FirstOrDefault().Name,
                Year= _unitOfWork.Month.FindAllAsync(m => m.GroupId == groupId).Result.OrderByDescending(m => m.Id).FirstOrDefault().Year,
                Days=viewDaysDto,
                MonthStudents=monthStudents
            };
            return Ok(monthData);
        }
        [Authorize]
        [HttpGet("GetMonthData")]
        public async Task<IActionResult> GetMonthDataAsync(int monthId)
        {
            if (await _unitOfWork.Month.GetByIdAsync(monthId) == null) return NotFound($"No month with id: {monthId}");
            var viewDaysDto = await _dayServices.GetAllAsync(monthId);
            foreach (var day in viewDaysDto)
            {
                day.studentAbsences = await _studentAbsenceService.GetAllAsync(day.Id);
            }
            var monthStudents = await _studentMonthService.GetAllAsync(monthId);

            var monthData = new ViewMonthDTO
            {
                Id = monthId,
                GroupId = _unitOfWork.Month.GetByIdAsync(monthId).Result.GroupId,
                Name= _unitOfWork.Month.GetByIdAsync(monthId).Result.Name,
                Year= _unitOfWork.Month.GetByIdAsync(monthId).Result.Year,
                Days = viewDaysDto,
                MonthStudents = monthStudents
            };
            return Ok(monthData);
        }
        [Authorize]

        [HttpGet("GetAllMonthsOfGroups")]
        public async Task<IActionResult> GetAllAsync([FromQuery] List<int> ids)
        {
            // If no ids are provided, return all months in the system.
            if (ids == null || ids.Count == 0)
            {
                var allMonths = await _unitOfWork.Month.FindAllAsync(g => true);
                return Ok(allMonths);
            }

            // Fetch months where the GroupId is in the list of ids.
            var months = await _unitOfWork.Month.FindAllAsync(g => ids.Contains(g.GroupId));

            if (months is null || !months.Any())
                return NotFound($"No groups found for the provided IDs.");

            return Ok(months);
        }
        [Authorize(Roles = "Teacher")]
        [HttpPost]
        public async Task<IActionResult> AddAsync(AddMonthDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if(await _unitOfWork.Group.GetByIdAsync(model.GroupId)==null)
                return BadRequest($"There is no group with id : {model.GroupId}");

            var month=_mapper.Map<Month>(model);

            if (await _unitOfWork.Month.CheckMonthExistAsync(month))
                return BadRequest("You try to repeat the month");

            try
            {
                await _unitOfWork.Month.AddAsync(month);
                await _unitOfWork.CompleteAsync();
                var monthDto =_mapper.Map<ViewMonthDTO>(month);
                await _studentMonthService.AddAsync(monthDto.Id);
                var monthStudents = await _studentMonthService.GetAllAsync(monthDto.Id);
                monthDto.MonthStudents = monthStudents;
                return Ok(monthDto);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.ToString());
            } 
        }
        [Authorize(Roles = "Teacher")]
        [HttpDelete]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            // Check if the month exists
            var month = await _unitOfWork.Month.GetByIdAsync(id);
            if (month == null)
                return BadRequest($"There is no month with ID: {id}");

            // Retrieve related days and month students
            var days = await _unitOfWork.Day.FindAllAsync(d => d.MonthId == id);
            var monthStudents = await _unitOfWork.StudentMonth.FindAllAsync(sm => sm.MonthId == id);

            // Begin transaction
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    // Delete days
                    if (days.Count() != 0)
                    {
                        foreach (var day in days)
                        {
                            var studentsAbsence= await _unitOfWork.StudentAbsence.FindAllAsync(sa=>sa.DayId== day.Id);
                            // Delete Student Absence
                            if(studentsAbsence.Count() != 0)
                            {
                                foreach(var studentAbsence in studentsAbsence)
                                {
                                    await _unitOfWork.StudentAbsence.DeleteAsync(studentAbsence);
                                }
                            }
                            await _unitOfWork.Day.DeleteAsync(day);
                        }
                    }

                    // Delete month students
                    if (monthStudents.Count()!=0)
                    {
                        foreach (var item in monthStudents)
                        {
                            await _unitOfWork.StudentMonth.DeleteAsync(item);
                        }
                    }

                    // Delete the month itself
                    await _unitOfWork.Month.DeleteAsync(month);

                    // Save all changes and commit transaction
                    await _unitOfWork.CompleteAsync();
                    await transaction.CommitAsync();

                    return Ok($"month of id: {id} is deleted successfully");
                }
                catch (Exception ex)
                {
                    // Rollback the transaction if any error occurs
                    await transaction.RollbackAsync();
                    return BadRequest(ex.Message);
                }
            }
        }
        [Authorize(Roles = "Teacher")]
        [HttpPut("SaveChanges")]
        public async Task<IActionResult> SaveAsync(SaveMonthDataDTO model)
        {
            SaveMonthDataDTO saved = new SaveMonthDataDTO();
            var studentAbsenceDTO = new List<StudentAbsenceDTO>();
            var studentMonthDto = new List<StudentMonthDto>();
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            foreach (var item in model.AbsenceStudents)
            {
                if (await _unitOfWork.Day.GetByIdAsync(item.DayId) == null)
                    return BadRequest($"No day with id: {item.DayId}");
                if (await _unitOfWork.Student.GetByIdAsync(item.StudentId) == null)
                    return BadRequest($"No student with id: {item.StudentId}");
            }
            foreach (var item in model.MonthStudents)
            {
                if (await _unitOfWork.Month.GetByIdAsync(item.MonthId) == null)
                    return BadRequest($"No Month with id: {item.MonthId}");
                if (await _unitOfWork.Student.GetByIdAsync(item.StudentId) == null)
                    return BadRequest($"No student with id: {item.StudentId}");
            }
            try
            {
                foreach(var item in model.AbsenceStudents)
                {
                    var updated = await _studentAbsenceService.UpdateAsync(item);
                    studentAbsenceDTO.Add(updated);
                
                }
                foreach(var item in model.MonthStudents)
                {
                    var updated = await _studentMonthService.UpdateAsync(item);
                    studentMonthDto.Add(updated);
                }
            }
            catch(Exception ex)
            {
                return BadRequest(ex);
            }
            saved.AbsenceStudents = studentAbsenceDTO;
            saved.MonthStudents = studentMonthDto;
            return Ok(saved);
        }
    }
}