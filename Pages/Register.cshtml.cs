using Locker;
using Locker.Data;
using Locker.Entities;
using Locker.Enums;
using Locker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace locker.Pages;

public class RegisterModel : BasePageModel
{
    public async Task OnGet()
    {
        
    }

    public async Task<IActionResult> OnPost(RegisterViewModel model)
    {
        using var _context = new LockerDbContext();
        var phoneCheck = await _context.Users.FirstOrDefaultAsync(x => x.Phone == model.Phone);
        if (phoneCheck != null)
        {
            //return with validation
        }

        var user = new User()
        {
            Name = model.Name,
            Phone = model.Phone,
            Email = model.Email,
            Password = model.Password.HashPassword(),
            Credit = 0,
            CreatedOn = DateTime.Now,
            Type = (int)UserType.Commercial,
            ModifiedOn = DateTime.Now,
            PromotionCredit = 0,
            
            
        };

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        return RedirectToPage("login");
        //return login page

    }
}