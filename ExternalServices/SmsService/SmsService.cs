
namespace Locker.ExternalServices;


public class SmsService : ISmsService
{
    public Task<bool> SendSms(string phoneNumber, string text)
    {
        return Task.FromResult(true);
    }
}