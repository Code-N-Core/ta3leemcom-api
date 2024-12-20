﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PlatformAPI.Core.Models;
using System.Reflection.Emit;

namespace PlatformAPI.EF.Data
{
    public class ApplicationDbContext:IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext() { }
        public ApplicationDbContext(DbContextOptions options):base(options) 
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<GroupQuiz>().HasKey(x => new {x.QuizId,x.GroupId});
            builder.Entity<StudentMonth>().HasKey(x => new { x.StudentId, x.MonthId });
            builder.Entity<StudentAbsence>().HasKey(x => new { x.StudentId, x.DayId });
            builder.Entity<TeacherNotification>().HasKey(x => new { x.TeacherId, x.NotificationId });
            builder.Entity<Quiz>()
            .Property(q => q.Duration)
            .HasConversion(
                v => v.Ticks,                    // Convert TimeSpan to Ticks for storage
                v => TimeSpan.FromTicks(v))      // Convert Ticks back to TimeSpan
            .HasColumnType("bigint")             // Store as BIGINT in the database
            .IsRequired();

        }
        public DbSet<Parent> Parents { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Level> Levels { get; set; }
        public DbSet<LevelYear> LevelYears { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Choose> Chooses { get; set; }
        public DbSet<StudentQuiz> StudentsQuizzes { get; set; }
        public DbSet<StudentAnswer> StudentAnswers { get; set; }
        public DbSet<GroupQuiz>GroupsQuizzes { get; set; }
        public DbSet<Month> Months { get; set; }
        public DbSet<Day> Days { get; set; }
        public DbSet<StudentMonth> StudentsMonths { get; set; }
        public DbSet<StudentAbsence> StudentsAbsences { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<TeacherNotification> TeachersNotifications { get; set;}
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Child> Children { get; set; }
        public DbSet<UserConnection> UserConnections { get; set; }
    }
}