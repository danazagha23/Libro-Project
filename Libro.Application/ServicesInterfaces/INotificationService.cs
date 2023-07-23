using Libro.Application.DTOs;
using Libro.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Application.ServicesInterfaces
{
    public interface INotificationService
    {
        Task<ICollection<NotificationDTO>> GetNotificationsForUserAsync(int userId);
        Task<int> GetUnreadNotificationCountAsync(int userId);
        Task CreateNotificationAsync(int userId, string message);
        Task MarkNotificationAsReadAsync(int notificationId);
    }
}
