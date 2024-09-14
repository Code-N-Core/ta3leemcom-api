using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PlatformAPI.Core.Hubs;
using PlatformAPI.Core.Interfaces;
using PlatformAPI.Core.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PlatformAPI.EF.Data;

namespace PlatformAPI.Core.Services
{
    public class QuizNotificationService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IHubContext<NotificationHub> _hubContext;

        public QuizNotificationService(IServiceProvider serviceProvider, IHubContext<NotificationHub> hubContext)
        {
            _serviceProvider = serviceProvider;
            _hubContext = hubContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await CheckEndedQuizzesAsync();
                await Task.Delay(3000, stoppingToken); // Check every 3 seconds
            }
        }

        private async Task CheckEndedQuizzesAsync()
        {
            // Create a scope for the ApplicationDbContext
            using (var scope = _serviceProvider.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var datenow = DateTime.Now;

                // Get ended quizzes
                var endedQuizzes =await _context.Quizzes
     .Include(q => q.GroupsQuizzes)
     .ThenInclude(gq => gq.Group)
     .ThenInclude(g => g.Teacher)
     .Where(q => !q.IsNotfy)  // Fetch non-notified quizzes first
     .ToListAsync();               // Get them all in memory first

                // Now manually filter in memory to debug and inspect
                var d=endedQuizzes.Last().EndDate;
                var filteredQuizzes = endedQuizzes
                    .Where(q => q.EndDate <= datenow)  // Client-side evaluation of the date logic
                    .ToList();


                foreach (var quiz in filteredQuizzes)
                {
                    foreach (var groupQuiz in quiz.GroupsQuizzes)
                    {
                        var teacher = groupQuiz.Group.Teacher;
                        if (teacher.IsSubscribed)
                        {
                            var message = $"Quiz {quiz.Title} has ended. Please check the results.";

                            // Send real-time notification to the teacher
                            await _hubContext.Clients.User(teacher.ApplicationUserId).SendAsync("ReceiveNotification", message);

                            // Save notification in the database
                            var notification = new Notification
                            {
                                Message = message,
                                Type = "Server",
                                IsReaded = false,
                                TeacherNotifications = new List<TeacherNotification>
                                {
                                    new TeacherNotification
                                    {
                                        TeacherId = teacher.Id,
                                        Date = DateTime.UtcNow,
                                        quizId = quiz.Id,
                                    }
                                }
                            };

                            await _context.Notifications.AddAsync(notification);
                        }
                    }

                    // Mark the quiz as notified
                    quiz.IsNotfy = true;
                }

                await _context.SaveChangesAsync();
            }
        }
    }
}
