using AutoMapper;
using PlatformAPI.Core.DTOs;
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
        }
    }
}