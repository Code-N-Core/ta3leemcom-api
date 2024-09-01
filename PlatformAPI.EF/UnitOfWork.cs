using Microsoft.EntityFrameworkCore.Storage;
using PlatformAPI.EF.Repositories;

namespace PlatformAPI.EF
{
    public class UnitOfWork : IUnitOfWork
    {
        private IDbContextTransaction _transaction;
        public ITeacherRepository Teacher {  get; private set; }

        public IStudentRepository Student {  get; private set; }

        public IParentRepository Parent {  get; private set; }

        public IQuizRepository Quiz {  get; private set; }

        public IQuestionRepository Question {  get; private set; }

        public IChooseRepository Choose {  get; private set; }

        public IGroupRepository Group {  get; private set; }

        public IMonthRepository Month {  get; private set; }

        public IStudentQuizRepository StudentQuiz {  get; private set; }

        public IStudentAbsenceRepository StudentAbsence {  get; private set; }

        public IStudentMonthRepository StudentMonth {  get; private set; }

        public IDayRepository Day {  get; private set; }

        public ILevelRepository Level {  get; private set; }

        public ILevelYearRepository LevelYear {  get; private set; }

        public IGroupQuizRepository GroupQuiz {  get; private set; }
        public IFeedbackRepository Feedback { get; private set; }

        private readonly ApplicationDbContext _context;
        public UnitOfWork(ApplicationDbContext context)
        {
            _context=context;
            Teacher=new TeacherRepository(context);
            Student=new StudentRepository(context);
            Parent=new ParentRepository(context);
            Quiz=new QuizRepository(context);
            Question=new QuestionRepository(context);
            Choose=new ChooseRepository(context);
            Group=new GroupRepository(context);
            Month=new MonthRepository(context);
            StudentQuiz=new StudentQuizRepository(context);
            StudentAbsence=new StudentAbsenceRepository(context);
            StudentMonth=new StudentMonthRepository(context);
            Day=new DayRepository(context);
            Level=new LevelRepository(context);
            LevelYear=new LevelYearRepository(context);
            GroupQuiz=new GroupQuizRepository(context);
            Feedback=new FeedbackRepository(context);
        }
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
            return _transaction;
        }
        public async Task<int> CompleteAsync()
        {
             return await _context.SaveChangesAsync();
        }

        public async Task RollbackAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task CommitAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }
}