using Microsoft.AspNetCore.SignalR;
namespace PlatformAPI.Core.Hubs
{
    public class NotificationHub : Hub
    {

        // Notify a specific teacher
        public async Task NotifyTeacher(string AppteacherId, string message)
        {
            // Send the real-time notification to the teacher
            await Clients.User(AppteacherId).SendAsync("ReceiveNotification", message);

        }
    }

}
