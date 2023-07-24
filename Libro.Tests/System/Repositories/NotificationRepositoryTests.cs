using Libro.Domain.Entities;
using Libro.Infrastructure.Data.DbContexts;
using Libro.Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Tests.System.Repositories
{
    public class NotificationRepositoryTests
    {
        private NotificationRepository _notificationRepository;
        private LibroDbContext _dbContext;
        private Mock<ILogger<NotificationRepository>> _logger;

        public NotificationRepositoryTests()
        {
            // Create an in-memory database for testing
            var options = new DbContextOptionsBuilder<LibroDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Use a unique database name
                .Options;

            _dbContext = new LibroDbContext(options);

            // Initialize the logger mock
            _logger = new Mock<ILogger<NotificationRepository>>();

            _notificationRepository = new NotificationRepository(null, _dbContext, _logger.Object);
        }

        [Fact]
        public async Task GetNotificationsForUserAsync_ShouldReturnNotificationsForUser()
        {
            // Arrange
            var userId = 1;
            var user = new User { UserId = userId, Username = "user1", Email = "user@gmail.com", FirstName = "user", LastName = "user", Password = "12345678" };
            var notifications = new List<Notification>
            {
                new Notification { Id = 1, UserId = userId, Message = "Notification 1", User = user },
                new Notification { Id = 2, UserId = userId, Message = "Notification 2", User = user },
                new Notification { Id = 3, UserId = userId, Message = "Notification 3", User = user }
            };

            _dbContext.Notifications.AddRange(notifications);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _notificationRepository.GetNotificationsForUserAsync(userId);

            // Assert
            Assert.Equal(notifications.Count, result.Count);
        }

        [Fact]
        public async Task GetUnreadNotificationCountAsync_ShouldReturnUnreadNotificationCount()
        {
            // Arrange
            var userId = 1;
            var notifications = new List<Notification>
            {
                new Notification { Id = 1, UserId = userId, IsRead = false, Message = "Notification 1" },
                new Notification { Id = 2, UserId = userId, IsRead = false, Message = "Notification 2"  },
                new Notification { Id = 3, UserId = userId, IsRead = true, Message = "Notification 3"  },
                new Notification { Id = 4, UserId = userId, IsRead = false, Message = "Notification 4"  }
            };

            _dbContext.Notifications.AddRange(notifications);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _notificationRepository.GetUnreadNotificationCountAsync(userId);

            // Assert
            Assert.Equal(3, result);
        }

        [Fact]
        public async Task CreateNotificationAsync_ShouldCreateNewNotification()
        {
            // Arrange
            var notification = new Notification
            {
                UserId = 1,
                Message = "New notification"
            };

            // Act
            await _notificationRepository.CreateNotificationAsync(notification);

            // Assert
            var createdNotification = await _dbContext.Notifications.FindAsync(notification.Id);
            Assert.NotNull(createdNotification);
            Assert.Equal(notification.Message, createdNotification.Message);
        }

        [Fact]
        public async Task GetNotificationByIdAsync_ShouldReturnNotificationWhenExists()
        {
            // Arrange
            var notification = new Notification
            {
                UserId = 1,
                Message = "Notification"
            };

            _dbContext.Notifications.Add(notification);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _notificationRepository.GetNotificationByIdAsync(notification.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(notification.Id, result.Id);
        }

        [Fact]
        public async Task UpdateNotificationAsync_ShouldUpdateExistingNotification()
        {
            // Arrange
            var notification = new Notification
            {
                UserId = 1,
                Message = "Notification"
            };

            _dbContext.Notifications.Add(notification);
            await _dbContext.SaveChangesAsync();

            // Update the notification message
            var updatedMessage = "Updated notification";
            notification.Message = updatedMessage;

            // Act
            await _notificationRepository.UpdateNotificationAsync(notification);

            // Assert
            var updatedNotification = await _dbContext.Notifications.FindAsync(notification.Id);
            Assert.NotNull(updatedNotification);
            Assert.Equal(updatedMessage, updatedNotification.Message);
        }
    }
}
