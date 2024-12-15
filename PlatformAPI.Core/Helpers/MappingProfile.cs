using AutoMapper;
using PlatformAPI.Core.DTOs.Auth;
using PlatformAPI.Core.DTOs.Choose;
using PlatformAPI.Core.DTOs.Day;
using PlatformAPI.Core.DTOs.Month;
using PlatformAPI.Core.DTOs.Questions;
using PlatformAPI.Core.Models;

namespace PlatformAPI.Core.Helpers
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<RegisterDTO, ApplicationUser>()
                .ForMember(dst => dst.PhoneNumberConfirmed, opt => opt.Ignore())
                .ForMember(dst => dst.EmailConfirmed, opt => opt.Ignore())
                .ForMember(dst => dst.AccessFailedCount, opt => opt.Ignore())
                .ForMember(dst => dst.ConcurrencyStamp, opt => opt.Ignore())
                .ForMember(dst => dst.Id, opt => opt.Ignore())
                .ForMember(dst => dst.LockoutEnabled, opt => opt.Ignore())
                .ForMember(dst => dst.LockoutEnd, opt => opt.Ignore())
                .ForMember(dst => dst.NormalizedEmail, opt => opt.Ignore())
                .ForMember(dst => dst.NormalizedUserName, opt => opt.Ignore())
                .ForMember(dst => dst.SecurityStamp, opt => opt.Ignore())
                .ForMember(dst => dst.TwoFactorEnabled, opt => opt.Ignore())
                .ForSourceMember(src => src.Role, opt => opt.DoNotValidate());
         /*   CreateMap<QDTO, Question>()
            .ForMember(dst => dst.Chooses, opt => opt.MapFrom(uq => uq.Choices));

            CreateMap<UQDTO, Question>()
                .ForMember(dst => dst.Chooses, opt => opt.MapFrom(uq=>uq.Choices))
                .ForMember(dst => dst.IsUpdated, opt => opt.Ignore());*/
            CreateMap<AddMonthDTO, Month>();
            CreateMap<Month, ViewMonthDTO>();
            CreateMap<DayDTO,Day>().ReverseMap();
            CreateMap<Day,ViewDayDTO>().ReverseMap();
        }
    }
}