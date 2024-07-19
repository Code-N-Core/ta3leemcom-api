namespace PlatformAPI.Core.Interfaces
{
    public interface IUnitOfWork
    {
        ITeacherRepository Teacher { get; }
        IStudentRepository Student { get; }
        IParentRepository Parent { get; }
        IQuizRepository Quiz { get; }
        IQuestionRepository Question { get; }
        IChooseRepository Choose { get; }
        IGroupRepository Group { get; }
        IMonthRepository Month { get; }
        IStudentQuizRepository StudentQuiz { get; }
        IStudentAbsenceRepository StudentAbsence { get; }
        IStudentMonthRepository StudentMonth { get; }
        IDayRepository Day { get; }
        ILevelRepository Level { get; }
        ILevelYearRepository LevelYear { get; }
        IGroupQuizRepository GroupQuiz { get; }
        int Complete();
    }
}