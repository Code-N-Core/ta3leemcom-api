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
            CreateMap<ChooseDTO, Choose>();
            CreateMap<CreateOnlineQuizDTO, Quiz>();
            CreateMap<CreateOffLineQuizDto, Quiz>();
            CreateMap<UpdateOnlineQuizDto, Quiz>();
            CreateMap<UpdateOfflineQuizDto, Quiz>();
            CreateMap<Quiz, ShowQuiz>();
            CreateMap<StudentMonthDto, StudentMonth>().ForMember(dest=>dest.Student,opt=>opt.Ignore())
                .ForMember(dest=>dest.Month,opt=>opt.Ignore()).ReverseMap();
            CreateMap<StudentAbsenceDTO, StudentAbsence>().ReverseMap();
        }
    }
}