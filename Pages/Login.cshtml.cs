using Locker;
using Locker.Data;
using Locker.Models;
using Locker.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace locker.Pages;



public class LoginModel(WorkContext workContext) : BasePageModel
{
    private readonly WorkContext _workContext = workContext;

  
    public async Task OnGet()
    {
        
    }


    public async Task<IActionResult> OnPost(LoginPageModel model)
    {
        await using var _context = new LockerDbContext();
        var user =
            await _context.Users.FirstOrDefaultAsync(x =>
                x.Phone == model.Phone && x.Password == model.Password.HashPassword());

        if (user == null) return null;

        _workContext.User = user;
        HttpContext.Session.SetInt32("IsLoggedIn",1);
        HttpContext.Session.SetString("Name",user.Name);
        HttpContext.Session.SetInt32("UserId",user.Id);
        HttpContext.Session.SetInt32("Role", user.Type);
        return RedirectToPage("AddressToLocker");

    }
    
}