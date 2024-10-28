using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using PlatformAPI.EF.Hubs;
using System.Linq;
using System.Threading;

namespace PlatformAPI.Core.Services
{
    public class QuizNotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IUnitOfWork _unitOfWork;
        private readonly QuizService _quizService;
        private readonly NotificationService _notificationService;
        private readonly ApplicationDbContext _context;
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);



        public QuizNotificationService(IHubContext<NotificationHub> hubContext, IUnitOfWork unitOfWork,
            QuizService quizService, NotificationService notificationService, ApplicationDbContext context)
        {
            _hubContext = hubContext;
            _unitOfWork = unitOfWork;
            _quizService = quizService;
            _notificationService = notificationService;
            _context = context;
        }

        public async Task CheckEndedQuizzesAsync()
        {
            // Use a lock to prevent concurrent execution
            await _semaphore.WaitAsync();
            try
            {
                var filteredQuizzes = await _quizService.GetEndedQuizez();
                if (filteredQuizzes is null)
                    return;

                foreach (var quiz in filteredQuizzes)
                {
                    if (quiz.IsNotfy) // Avoid duplicate notifications
                        continue;

                    var teacher = quiz.Teacher;
                    var message = $"نتيجه اختبارك في {quiz.Title} اصبحت متاحه. يمكنك مراجعه درجاتك الان";

                    await SendNotificationToUser(teacher.ApplicationUserId, message);
                    await _notificationService.SaveN(message, teacher.Id, quiz.Id);

                    quiz.IsNotfy = true;
                    _unitOfWork.Quiz.Update(quiz);
                }

                await _unitOfWork.CompleteAsync();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task SendNotificationToUser(string userId, string message)
        {
            // Retrieve all connection IDs for the user from your connection mapping table
            var connectionIds = await _context.UserConnections
                .Where(cm => cm.UserId == userId)
                .Select(cm => cm.ConnectionId)
                .ToListAsync();

            // Send message to all connections
            foreach (var connectionId in connectionIds)
            {
                await _hubContext.Clients.Client(connectionId).SendAsync("ReceiveNotification", message);
            }
        }
    }
}
