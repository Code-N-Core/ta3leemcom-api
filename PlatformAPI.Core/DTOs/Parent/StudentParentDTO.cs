namespace PlatformAPI.Core.DTOs.Parent
{
    public class StudentParentDTO
    {
        // Student Data
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string LevelName { get; set; }
        public string LevelYearName { get; set; }
        public string GroupName { get; set; }
        // Student Months Data
        public IEnumerable<StudentMonthParentDTO>? Months { get; set; }
        // Student Quizzes
        public IEnumerable<StudentQuizParentDTO>? Quizzes { get; set; }
        // Teacher Data
        public string TeacherName {  get; set; }
        public string TeacherPhone { get; set; }
        public string TeacherEmail { get; set; }
    }
}