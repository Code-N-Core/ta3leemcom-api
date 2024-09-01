using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformAPI.Core.DTOs.Day;
using PlatformAPI.Core.Interfaces;
using PlatformAPI.Core.Models;

namespace PlatformAPI.Core.Services
{
    public class DayServices:IDayServices
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IMapper _mapper;
        public DayServices(IUnitOfWork unitOfWork,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ViewDayDTO>> GetAllAsync(int monthId)
        {
            var days = await _unitOfWork.Day.FindAllAsync(d => d.MonthId == monthId);
            var viewDaysDtos = new List<ViewDayDTO>();
            foreach (var day in days)
            {
                var viewDayDTO = _mapper.Map<ViewDayDTO>(day);
                viewDaysDtos.Add(viewDayDTO);
            }
            return viewDaysDtos;
        }
    }
}
