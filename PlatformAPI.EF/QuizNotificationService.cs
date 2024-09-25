using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using PlatformAPI.Core.Hubs;

namespace PlatformAPI.Core.Services
{
    public class QuizNotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IUnitOfWork _unitOfWork;
        private readonly QuizService _quizService;
        private readonly NotificationService _notificationService;

        public QuizNotificationService(IHubContext<NotificationHub> hubContext, IUnitOfWork unitOfWork,
            QuizService quizService, NotificationService notificationService)
        {
            _hubContext = hubContext;
            _unitOfWork = unitOfWork;
            _quizService = quizService;
            _notificationService = notificationService;
        }



        // This method will be scheduled by Hangfire as a recurring job
        public async Task CheckEndedQuizzesAsync()
        {
                var filteredQuizzes = await _quizService.GetEndedQuizez();

                foreach (var quiz in filteredQuizzes)
                {
                    foreach (var groupQuiz in quiz.GroupsQuizzes)
                    {
                        var teacher = groupQuiz.Group.Teacher;
                        if (teacher.IsSubscribed)
                        {
                            var message = $"Quiz (id: {quiz.Id}) => {quiz.Title} has ended. Please check the results.";

                            // Send real-time notification to the teacher
                            await _hubContext.Clients.User(teacher.ApplicationUserId).SendAsync("ReceiveNotification", message);

                            // Save notification in the database
                            var not = _notificationService.SaveN(message, teacher.Id, quiz.Id);
                        }
                    }

                    // Mark the quiz as notified
                    quiz.IsNotfy = true;
                }

                await _unitOfWork.CommitAsync();
            
        }
    }
}
