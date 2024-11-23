namespace Locker.ExternalServices;


public interface ISmsService{
    Task<bool> SendSms(string phoneNumber, string text);
}