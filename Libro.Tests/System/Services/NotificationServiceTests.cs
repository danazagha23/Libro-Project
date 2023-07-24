using AutoMapper;
using Libro.Application.DTOs;
using Libro.Application.Services;
using Libro.Application.ServicesInterfaces;
using Libro.Domain.Entities;
using Libro.Domain.RepositoriesInterfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Libro.Tests.Services
{
    public class NotificationServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly Mock<INotificationRepository> _notificationRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly NotificationService _notificationService;

        public NotificationServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _emailServiceMock = new Mock<IEmailService>();
            _notificationRepositoryMock = new Mock<INotificationRepository>();
            _mapperMock = new Mock<IMapper>();
            _notificationService = new NotificationService(_userRepositoryMock.Object, _emailServiceMock.Object, _notificationRepositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetNotificationsForUserAsync_WithValidUserId_ShouldReturnNotifications()
        {
            // Arrange
            var userId = 1;
            var notifications = new List<Notification>
            {
                new Notification { Id = 1, UserId = userId, Message = "Notification 1" },
                new Notification { Id = 2, UserId = userId, Message = "Notification 2" }
            };
            var notificationDTOs = new List<NotificationDTO>
            {
                new NotificationDTO { Id = 1, UserId = userId, Message = "Notification 1" },
                new NotificationDTO { Id = 2, UserId = userId, Message = "Notification 2" }
            };

            _notificationRepositoryMock.Setup(repo => repo.GetNotificationsForUserAsync(userId)).ReturnsAsync(notifications);
            _mapperMock.Setup(mapper => mapper.Map<ICollection<NotificationDTO>>(notifications)).Returns(notificationDTOs);

            // Act
            var result = await _notificationService.GetNotificationsForUserAsync(userId);

            // Assert
            Assert.Equal(notificationDTOs, result);

            _notificationRepositoryMock.Verify(repo => repo.GetNotificationsForUserAsync(userId), Times.Once);
            _mapperMock.Verify(mapper => mapper.Map<ICollection<NotificationDTO>>(notifications), Times.Once);
        }

        [Fact]
        public async Task GetUnreadNotificationCountAsync_WithValidUserId_ShouldReturnUnreadNotificationCount()
        {
            // Arrange
            var userId = 1;
            var unreadCount = 2;

            _notificationRepositoryMock.Setup(repo => repo.GetUnreadNotificationCountAsync(userId)).ReturnsAsync(unreadCount);

            // Act
            var result = await _notificationService.GetUnreadNotificationCountAsync(userId);

            // Assert
            Assert.Equal(unreadCount, result);

            _notificationRepositoryMock.Verify(repo => repo.GetUnreadNotificationCountAsync(userId), Times.Once);
        }

        [Fact]
        public async Task CreateNotificationAsync_WithValidUserIdAndMessage_ShouldCreateNotificationAndSendEmail()
        {
            // Arrange
            var userId = 1;
            var message = "New notification";
            var user = new User { UserId = userId, Email = "test@example.com" };
            var notification = new Notification { UserId = userId, Message = message, IsRead = false, CreatedAt = DateTime.Now, User = user, Id = 1};

            _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(userId)).ReturnsAsync(user);
            _emailServiceMock.Setup(service => service.SendEmail(user.Email, "Libro Notification", message)).Returns(Task.CompletedTask);
            _notificationRepositoryMock.Setup(repo => repo.CreateNotificationAsync(notification)).Returns(Task.CompletedTask);

            // Act
            await _notificationService.CreateNotificationAsync(userId, message);

            // Assert
            _userRepositoryMock.Verify(repo => repo.GetUserByIdAsync(userId), Times.Once);
            _emailServiceMock.Verify(service => service.SendEmail(user.Email, "Libro Notification", message), Times.Once);
            _notificationRepositoryMock.Verify(repo => repo.CreateNotificationAsync(It.IsAny<Notification>()), Times.Once);
        }

        [Fact]
        public async Task MarkNotificationAsReadAsync_WithValidNotificationId_ShouldMarkNotificationAsRead()
        {
            // Arrange
            var notificationId = 1;
            var notification = new Notification { Id = notificationId, UserId = 1, Message = "Notification", IsRead = false };

            _notificationRepositoryMock.Setup(repo => repo.GetNotificationByIdAsync(notificationId)).ReturnsAsync(notification);
            _notificationRepositoryMock.Setup(repo => repo.UpdateNotificationAsync(notification)).Returns(Task.CompletedTask);

            // Act
            await _notificationService.MarkNotificationAsReadAsync(notificationId);

            // Assert
            _notificationRepositoryMock.Verify(repo => repo.GetNotificationByIdAsync(notificationId), Times.Once);
            _notificationRepositoryMock.Verify(repo => repo.UpdateNotificationAsync(notification), Times.Once);

            Assert.True(notification.IsRead);
        }
    }
}
