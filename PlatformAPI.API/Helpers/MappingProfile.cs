using PlatformAPI.Core.DTOs.Choose;
using PlatformAPI.Core.DTOs.Groub;
using PlatformAPI.Core.DTOs.Questions;
using PlatformAPI.Core.DTOs.Quiz;
using PlatformAPI.Core.DTOs.Student;
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
            CreateMap<ChooseDTO, Choose>();
            CreateMap<CreateOnlineQuizDTO, Quiz>();
            CreateMap<CreateOffLineQuizDto, Quiz>();
            CreateMap<UpdateOnlineQuizDto, Quiz>();
            CreateMap<UpdateOfflineQuizDto, Quiz>();
            CreateMap<Quiz, ShowQuiz>();
        }
    }
}