using Locker.Entities;

namespace Locker.Services;

public interface IAuthService
{
    string CreateToken(User user);
}