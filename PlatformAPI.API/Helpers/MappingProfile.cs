using PlatformAPI.Core.DTOs.Choose;
using PlatformAPI.Core.DTOs.Groub;
using PlatformAPI.Core.DTOs.Questions;
using PlatformAPI.Core.DTOs.Quiz;
using PlatformAPI.Core.DTOs.Student;
using PlatformAPI.Core.DTOs.StudentAbsence;
using PlatformAPI.Core.DTOs.StudentMonth;
using Group = PlatformAPI.Core.Models.Group;

namespace PlatformAPI.API.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<AddGroupDTO, PlatformAPI.Core.Models.Group>();
            CreateMap<CreateStudentDTO, Student>();
            CreateMap<Student, StudentDTO>();
            CreateMap<Group,GroupDTO>();
            CreateMap<AddFeedbackDTO, Feedback>();
            CreateMap<ChooseDTO, Choose>().ReverseMap();
            CreateMap<CreateOnlineQuizDTO, Quiz>().ForMember(dest=>dest.Questions,opt=>opt.Ignore());
            CreateMap<CreateOffLineQuizDto, Quiz>();
            CreateMap<UpdateOnlineQuizDto, Quiz>();
            CreateMap<UQDTO, Question>();
            CreateMap<UpdateOfflineQuizDto, Quiz>();
            CreateMap<Quiz, ShowQuiz>()
                .ForMember(dest => dest.timeStart, opt => opt.MapFrom(src => new timeStart
                {
                    Hours = src.StartDate.Hour,
                    Minute = src.StartDate.Minute,
                    Mode = src.StartDate.Hour >= 12 ? "PM" : "AM"
                }))
                .ForMember(dest => dest.timeDuration, opt => opt.MapFrom(src => new timeDuration
                {
                    Hours = src.Duration.Hours,
                    Minute = src.Duration.Minutes,
                    Days = src.Duration.Days > 0 ? src.Duration.Days : 0,
                    Mode = src.Duration.Hours >= 12 ? "PM" : "AM"
                }))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate)); // EndDate is already computed in Quiz class
            CreateMap<StudentMonthDto, StudentMonth>().ForMember(dest=>dest.Student,opt=>opt.Ignore())
                .ForMember(dest=>dest.Month,opt=>opt.Ignore()).ReverseMap();
            CreateMap<StudentAbsenceDTO, StudentAbsence>().ReverseMap();
        }
    }
}