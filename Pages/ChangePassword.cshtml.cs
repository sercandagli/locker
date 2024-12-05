using Locker;
using Locker.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace locker.Pages;

public class ChangePasswordModel(WorkContext workContext) : BasePageModel
{
    public bool IsForgotPasswordProcess { get; set; }
    private readonly WorkContext _workContext = workContext;

    public async Task<IActionResult> OnGet()
    {
        if (_workContext.User == null)
            return RedirectToPage("login");

        IsForgotPasswordProcess = _workContext.User.ForgotPasswordProcess;
        return Page();
    }


    public async Task<IActionResult> OnPost(ChangePasswordViewModel model)
    {
        if (_workContext.User == null)
            return RedirectToPage("login");

        await using var _context = new LockerDbContext();
        var user =
            await _context.Users.FirstOrDefaultAsync(x =>
                x.Id == _workContext.User.Id);

        if (user == null) return null;
        var isValid = false;
        if (model.Password != model.VerifyPassword) return RedirectToPage("changePassword");
        
        if (user.ForgotPasswordProcess)
        {
            user.Password = model.Password.HashPassword();
            user.ModifiedOn = DateTime.Now;
            isValid = true;
        }
        else if (user.Password == model.OldPassword.HashPassword())
        {
            user.Password = model.Password.HashPassword();
            user.ModifiedOn = DateTime.Now;
            isValid = true;
        }

        if (!isValid) return RedirectToPage("changePassword");
        
        await _context.SaveChangesAsync();
        HttpContext.Session.SetInt32("IsLoggedIn", 0);
        HttpContext.Session.SetString("Name", string.Empty);
        HttpContext.Session.SetInt32("UserId", 0);
        HttpContext.Session.SetInt32("Role", 0);
        return RedirectToPage("login");


    }
}