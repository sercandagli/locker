using Locker;
using Locker.Data;
using Locker.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace locker.Pages;

public class ManagementLoginModel : BasePageModel
{
    
    private readonly WorkContext _workContext;

    public ManagementLoginModel(WorkContext workContext)
    {
        _workContext = workContext;
    }
    
    public async Task<IActionResult> OnPost(Courier courierModel)
    {
        using var _context = new LockerDbContext();
        var admin = await _context.Admins.FirstOrDefaultAsync(x =>
            x.Phone == courierModel.Phone && x.Password == courierModel.Password.HashPassword());

        if (admin != null)
        {
            HttpContext.Session.SetInt32("IsLoggedIn",1);
            HttpContext.Session.SetInt32("AdminId",admin.Id);
            HttpContext.Session.SetInt32("CourierId",0);

            _workContext.Admin = admin;
            return RedirectToPage("orders");
        }

        return RedirectToPage("managementLogin");
    }
}