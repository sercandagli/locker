using System.Text.Json;
using Locker.Data;
using Locker.Models;
using locker.Pages;
using Microsoft.EntityFrameworkCore;

namespace Locker.Middlewares;

public class AuthenticationMiddleware : IMiddleware
{

    private readonly WorkContext _context;

    public AuthenticationMiddleware(WorkContext context,
        BasePageModel model)
    {
        _context = context;
    }
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (context.Session.GetInt32("IsLoggedIn") == 1)
        {
            using var _context = new LockerDbContext();

            if (context.Session.GetInt32("UserId") is not null && context.Session.GetInt32("UserId") != 0)
            {
                this._context.User =
                    await _context.Users.FirstOrDefaultAsync(x => x.Id == context.Session.GetInt32("UserId"));
            }else if (context.Session.GetInt32("CourierId") is not null && context.Session.GetInt32("CourierId") != 0)
            {
                this._context.Courier = await _context.Couriers.FirstOrDefaultAsync( x=> x.Id == context.Session.GetInt32("CourierId"));
            }else if (context.Session.GetInt32("AdminId") is not null && context.Session.GetInt32("AdminId") != 0)
            {
                this._context.Admin = await _context.Admins.FirstOrDefaultAsync( x=> x.Id == context.Session.GetInt32("AdminId"));
            }
            

        }

        await next.Invoke(context);

    }
}

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseAuthenticationMiddleware(
        this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<AuthenticationMiddleware>();
    }
}