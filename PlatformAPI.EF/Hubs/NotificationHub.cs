using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using PlatformAPI.Core.Models;
using System.Security.Claims;
namespace PlatformAPI.EF.Hubs
{
    public class NotificationHub : Hub
    {
        private readonly ApplicationDbContext _context;

        public NotificationHub(ApplicationDbContext context)
        {
            _context = context;
        }

        // Notify a specific teacher
        public async Task SendNotificationToUser(string userId, string message)
        {
            // Retrieve all connection IDs for the user
            var connectionIds = await _context.UserConnections
                .Where(cm => cm.UserId == userId)
                .Select(cm => cm.ConnectionId)
                .ToListAsync();

            // Send message to all connections
            foreach (var connectionId in connectionIds)
            {
                await Clients.Client(connectionId).SendAsync("ReceiveNotification", message);
            }
        }


        public override async Task OnConnectedAsync()
        {
            var userId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value; // Get user ID from JWT
            var connectionId = Context.ConnectionId; // Get the current connection ID

            // Add to connection mapping table
            var connectionMapping = new UserConnection { UserId = userId, ConnectionId = connectionId };
            _context.UserConnections.Add(connectionMapping);
            await _context.SaveChangesAsync();

            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var connectionId = Context.ConnectionId;
            var userId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Remove from connection mapping table
            var mapping = await _context.UserConnections
                .FirstOrDefaultAsync(cm => cm.UserId == userId && cm.ConnectionId == connectionId);

            if (mapping != null)
            {
                _context.UserConnections.Remove(mapping);
                await _context.SaveChangesAsync();
            }

            await base.OnDisconnectedAsync(exception);
        }

    }

}
