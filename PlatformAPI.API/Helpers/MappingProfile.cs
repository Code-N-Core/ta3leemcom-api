﻿using Group = PlatformAPI.Core.Models.Group;

namespace PlatformAPI.API.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<AddGroupDTO, PlatformAPI.Core.Models.Group>();
            CreateMap<CreateStudentDTO, Student>();
            CreateMap<Group,GroupDTO>();
        }
    }
}