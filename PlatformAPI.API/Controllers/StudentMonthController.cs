using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlatformAPI.Core.DTOs.StudentMonth;
using PlatformAPI.Core.Models;
using Group = PlatformAPI.Core.Models.Group;

namespace PlatformAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentMonthController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public StudentMonthController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        //[HttpGet]
        //public async Task<IActionResult> GetAllAsync(int monthId)
        //{
        //    if (await _unitOfWork.StudentMonth.FindTWithExpression<Month>(m=>m.Id==monthId) == null)
        //        return NotFound($"No month with id: {monthId}");
        //    var model = await _unitOfWork.StudentMonth.FindAllAsync(sm=>sm.MonthId==monthId);
        //    List<StudentMonthDto> studentsMonths=new List<StudentMonthDto>();
        //    foreach (var studentMonth in model)
        //    {
        //        studentsMonths.Add(_mapper.Map<StudentMonthDto>(studentMonth));
        //    }
        //    return Ok(studentsMonths);
        //}

        //[HttpPost("AddMonthStudents")]
        //public async Task<IActionResult> AddAsync([FromQuery]int monthId)
        //{
        //    if(await _unitOfWork.Month.GetByIdAsync(monthId)==null)
        //        return NotFound($"No month with id: {monthId}");

        //    if (_unitOfWork.StudentMonth.FindAllAsync(sm => sm.MonthId == monthId).Result.Count()!=0)
        //        return BadRequest("You try to repeat these students month");

        //    var groupId =_unitOfWork.Month.GetByIdAsync(monthId).Result.GroupId;
        //    var students=await _unitOfWork.Student.FindAllAsync(s=>s.GroupId == groupId);
        //    foreach(var student in students)
        //    {
        //        var model = new StudentMonth { StudentId = student.Id, MonthId = monthId, Pay = false };
        //        try
        //        {
        //            await _unitOfWork.StudentMonth.AddAsync(model);
        //        }
        //        catch (Exception ex)
        //        {
        //            return BadRequest(ex);
        //        }
        //    }
        //    await _unitOfWork.CompleteAsync();
        //    return Ok("MonthStudents added successfully");
        //}
        [HttpPut]
        public async Task<IActionResult> UpdateAsync(List<StudentMonthDto> models)
        {
            foreach(var model in models)
            {
                var existingEntity = await _unitOfWork.StudentMonth.FindTWithExpression<StudentMonth>(
                                    sm => sm.MonthId == model.MonthId && sm.StudentId == model.StudentId);

                if (existingEntity == null)
                    return NotFound($"No student month with studentId: {model.StudentId}, and monthId: {model.MonthId}");
                try
                {
                    _mapper.Map(model, existingEntity);
                    _unitOfWork.StudentMonth.Update(existingEntity);
                }
                catch(Exception ex)
                {
                    return BadRequest(ex);
                }
            }
            await _unitOfWork.CompleteAsync();
            return Ok("Month students updated successfully");
        }
    }
}