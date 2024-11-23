namespace Locker.Services;



public class NotificationService : INotificationService
{
    public Task SendOrderCreatedNotification(string phoneNumber)
    {
        return Task.FromResult(true);
    }

    public Task<string> SendOtp(string phoneNumber)
    {
        return Task.FromResult(new Random().Next(1000,9999).ToString());
    }
}