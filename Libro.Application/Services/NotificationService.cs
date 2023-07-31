using AutoMapper;
using Libro.Application.DTOs;
using Libro.Application.ServicesInterfaces;
using Libro.Domain.Entities;
using Libro.Domain.RepositoriesInterfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IEmailService _emailService;
        private readonly INotificationRepository _notificationRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(IUserRepository userRepository, IEmailService emailService, INotificationRepository notificationRepository, IMapper mapper, ILogger<NotificationService> logger)
        {
            _emailService = emailService;
            _userRepository = userRepository;
            _notificationRepository = notificationRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ICollection<NotificationDTO>> GetNotificationsForUserAsync(int userId)
        {
            try
            {
                return _mapper.Map<ICollection<NotificationDTO>>(await _notificationRepository.GetNotificationsForUserAsync(userId));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while getting notifications for user with ID {userId}.");
                throw;
            }
        }

        public async Task<int> GetUnreadNotificationCountAsync(int userId)
        {
            try
            {
                return await _notificationRepository.GetUnreadNotificationCountAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while getting unread notification count for user with ID {userId}.");
                throw;
            }
        }

        public async Task CreateNotificationAsync(int userId, string message)
        {
            try
            {
                var notification = new Notification
                {
                    UserId = userId,
                    Message = message,
                    IsRead = false,
                    CreatedAt = DateTime.Now,
                    User = await _userRepository.GetUserByIdAsync(userId)
                };
                // Send email notification
                await _emailService.SendEmail(notification.User.Email, "Libro Notification", notification.Message);
                await _notificationRepository.CreateNotificationAsync(notification);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while creating a notification for user with ID {userId}.");
                throw;
            }
        }

        public async Task MarkNotificationAsReadAsync(int notificationId)
        {
            try
            {
                var notification = await _notificationRepository.GetNotificationByIdAsync(notificationId);

                if (notification != null)
                {
                    notification.IsRead = true;
                    await _notificationRepository.UpdateNotificationAsync(notification);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while marking notification with ID {notificationId} as read.");
                throw;
            }
        }
    }
}
