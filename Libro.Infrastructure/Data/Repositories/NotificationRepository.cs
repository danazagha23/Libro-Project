using Libro.Application.ServicesInterfaces;
using Libro.Domain.Entities;
using Libro.Domain.RepositoriesInterfaces;
using Libro.Infrastructure.Data.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Infrastructure.Data.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly LibroDbContext _context;
        private readonly ILogger<NotificationRepository> _logger;

        public NotificationRepository(IEmailService emailService, LibroDbContext context, ILogger<NotificationRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ICollection<Notification>> GetNotificationsForUserAsync(int userId)
        {
            try
            {
                _logger.LogInformation("Fetching notifications for user ID: {UserId} from the database.", userId);
                return await _context.Notifications
                    .Include(u => u.User)
                    .Where(n => n.UserId == userId)
                    .OrderByDescending(n => n.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching notifications for user ID: {UserId}.", userId);
                throw;
            }
        }

        public async Task<int> GetUnreadNotificationCountAsync(int userId)
        {
            try
            {
                _logger.LogInformation("Fetching unread notification count for user ID: {UserId} from the database.", userId);
                return await _context.Notifications
                    .Include(u => u.User)
                    .CountAsync(n => n.UserId == userId && !n.IsRead);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching unread notification count for user ID: {UserId}.", userId);
                throw;
            }
        }

        public async Task CreateNotificationAsync(Notification notification)
        {
            try
            {
                _logger.LogInformation("Creating a new notification in the database.");
                await _context.Notifications.AddAsync(notification);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Notification created and email sent successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating a new notification.");
                throw;
            }
        }

        public async Task<Notification> GetNotificationByIdAsync(int notificationId)
        {
            try
            {
                _logger.LogInformation("Fetching notification by ID: {NotificationId} from the database.", notificationId);
                return await _context.Notifications.FindAsync(notificationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching notification by ID: {NotificationId}.", notificationId);
                throw;
            }
        }

        public async Task UpdateNotificationAsync(Notification notification)
        {
            try
            {
                _logger.LogInformation("Updating notification with ID: {NotificationId} in the database.", notification.Id);
                _context.Notifications.Update(notification);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating notification with ID: {NotificationId}.", notification.Id);
                throw;
            }
        }
    }
}
