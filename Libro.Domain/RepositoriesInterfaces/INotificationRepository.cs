using Libro.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Domain.RepositoriesInterfaces
{
    public interface INotificationRepository
    {
        Task<List<Notification>> GetNotificationsForUserAsync(int userId);
        Task<int> GetUnreadNotificationCountAsync(int userId);
        Task CreateNotificationAsync(Notification notification);
        Task<Notification> GetNotificationByIdAsync(int notificationId);
        Task UpdateNotificationAsync(Notification notification);
    }
}
