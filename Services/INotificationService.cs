namespace Locker.Services;



public interface INotificationService {
    Task<string> SendOtp(string phoneNumber);

    Task SendOrderCreatedNotification(string phoneNumber);
}