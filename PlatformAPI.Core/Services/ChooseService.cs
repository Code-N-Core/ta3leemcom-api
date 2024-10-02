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

        public async Task<bool> CreateChoose(List<ChooseDTO> models)
        {
            Choose choice = null;
            try
            {
                // Map the DTO to the entity
                foreach (var model in models)
                {
                    choice = _mapper.Map<Choose>(model);

                    // Add the choice to the repository
                    await _unitOfWork.Choose.AddAsync(choice);
                }
               

                // Commit the changes
            }
            catch (Exception ex)
            {
                return false; 
            }
            await _unitOfWork.CompleteAsync();

            return true; 
        }

    }
}
