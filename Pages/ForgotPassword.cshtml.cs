using Locker;
using Locker.Data;
using Locker.Models;
using Locker.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace locker.Pages;



public class ForgotPasswordModel(WorkContext workContext) : BasePageModel
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
                x.Phone == model.Phone.FormatPhone());

        if (user == null){
            this.Message = "Bu telefona ait kullanıcı bulunamadı";
            return Page();
        }

        var rnd = new Random();
        var pass = rnd.Next(6);

        user.Password = pass.ToString().HashPassword();
        user.ForgotPasswordProcess = true;
        user.ModifiedOn = DateTime.Now;

        await _context.SaveChangesAsync();
        
        return RedirectToPage("login");

    }
    
}