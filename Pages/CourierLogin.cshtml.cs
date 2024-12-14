using Locker;
using Locker.Data;
using Locker.Entities;
using Microsoft.AspNetCore.DataProtection.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace locker.Pages;

public class CourierLoginModel : BasePageModel
{
    private readonly WorkContext _workContext;

    public CourierLoginModel(WorkContext workContext)
    {
        _workContext = workContext;
    }

    public async Task OnGet()
    {
        
    }

    public async Task<IActionResult> OnPost(Courier courierModel)
    {
        using var _context = new LockerDbContext();
        var courier = await _context.Couriers.FirstOrDefaultAsync(x =>
            x.Phone == courierModel.Phone.FormatPhone() && x.Password == courierModel.Password.HashPassword());

        if (courier != null)
        {
            HttpContext.Session.SetInt32("IsLoggedIn",1);
            HttpContext.Session.SetInt32("CourierId",courier.Id);
            HttpContext.Session.SetInt32("AdminId",0);

            _workContext.Courier = courier;
            return RedirectToPage("courierOrders");
        }

        return RedirectToPage("courierLogin");
    }
}