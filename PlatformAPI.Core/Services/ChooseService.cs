using AutoMapper;
using PlatformAPI.Core.DTOs.Choose;
using PlatformAPI.Core.Interfaces;
using PlatformAPI.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatformAPI.Core.Services
{
    public class ChooseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ChooseService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Choose> CreateChoose(ChooseDTO model)
        {
            var choice = new Choose();
            try
            {
                 choice = _mapper.Map<Choose>(model);
                await _unitOfWork.Choose.AddAsync(choice);
               
            }
            catch (Exception ex)
            {

                return null;
            }
            await _unitOfWork.CompleteAsync();
            return choice;
        }
    }
}
