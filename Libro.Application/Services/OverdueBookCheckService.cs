using Libro.Application.ServicesInterfaces;
using Libro.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class OverdueBookCheckService : IHostedService, IDisposable
{
    private readonly IServiceScopeFactory _scopeFactory;
    private Timer _timer;

    public OverdueBookCheckService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        // Set up the timer to execute the CheckOverdueBooks method every 12 hours
        _timer = new Timer(async (_) => await CheckOverdueBooks(), null, TimeSpan.Zero, TimeSpan.FromHours(12));
        return Task.CompletedTask;
    }

    private async Task CheckOverdueBooks()
    {
        try
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var bookTransactionsService = scope.ServiceProvider.GetRequiredService<IBookTransactionsService>();
                var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

                var allTrans = await bookTransactionsService.GetAllBookTransactionsAsync();
                // Get the list of overdue books
                var overdueBooks = allTrans.Where(t => t.TransactionType == TransactionType.Borrowed && !t.IsReturned && t.DueDate <= DateTime.Now);

                // Process each overdue book and send notifications
                foreach (var book in overdueBooks)
                {
                    var message = $"The book with title {book.Book.Title} is overdue. Please return it as soon as possible.";
                    await notificationService.CreateNotificationAsync(book.PatronId, message);
                }
            }
        }
        catch (Exception ex)
        {
            // Handle any potential exceptions that may occur during the overdue book check
            // You can log the error, notify administrators, or take any other necessary actions.
            Console.WriteLine($"An error occurred while checking for overdue books: {ex.Message}");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
