using AutoMapper;
using Libro.Application.DTOs;
using Libro.Application.ServicesInterfaces;
using Libro.Domain.Entities;
using Libro.Domain.RepositoriesInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IMapper _mapper;

        public NotificationService(INotificationRepository notificationRepository, IMapper mapper)
        {
            _notificationRepository = notificationRepository;
            _mapper = mapper;
        }

        public async Task<List<NotificationDTO>> GetNotificationsForUserAsync(int userId)
        {
            return _mapper.Map<List<NotificationDTO>>(await _notificationRepository.GetNotificationsForUserAsync(userId));
        }

        public async Task<int> GetUnreadNotificationCountAsync(int userId)
        {
            return await _notificationRepository.GetUnreadNotificationCountAsync(userId);
        }

        public async Task CreateNotificationAsync(int userId, string message)
        {
            var notification = new Notification
            {
                UserId = userId,
                Message = message,
                IsRead = false,
                CreatedAt = DateTime.Now
            };

            await _notificationRepository.CreateNotificationAsync(notification);
        }

        public async Task MarkNotificationAsReadAsync(int notificationId)
        {
            var notification = await _notificationRepository.GetNotificationByIdAsync(notificationId);

            if (notification != null)
            {
                notification.IsRead = true;
                await _notificationRepository.UpdateNotificationAsync(notification);
            }
        }
    }
}
