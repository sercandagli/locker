using Locker.Data;
using Locker.Entities;
using Locker.Enums;
using locker.Pages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Locker.Pages;


public class AddVIPPartnerModel : BasePageModel
{


    private readonly WorkContext _workContext;

    public AddVIPPartnerModel(WorkContext workContext)
    {
        _workContext = workContext;
    }

    public void OnGet()
    {

        if (_workContext.Admin == null)
            return;
    }


    public async Task<IActionResult> OnPost(User user){
        
        if (_workContext.Admin == null)
            return RedirectToPage("managementLogin");
        using var _context = new LockerDbContext();
        user.CreatedOn = DateTime.Now;
        user.ModifiedOn = DateTime.Now;
        user.Type = (int)UserType.Partner;
        user.Password = user.Password.HashPassword();
        _context.Users.Add(user);

        await _context.SaveChangesAsync();
        return RedirectToPage("userList");
    }

}