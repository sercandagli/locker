using Locker;
using Locker.Data;
using Locker.Entities;
using Locker.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace locker.Pages;

public class CouriersModel : BasePageModel
{

    private readonly WorkContext _workContext;

    public CouriersModel(WorkContext workContext)
    {
        _workContext = workContext;
    }

    public List<Courier> Couriers{get;set;}
    public async Task<IActionResult> OnGet()
    {
        if (_workContext.Admin == null)
            return RedirectToPage("managementLogin");
        using var _context = new LockerDbContext();
        Couriers = await _context.Couriers.Include(x => x.Region).ToListAsync();
        return Page();

    }

    public async Task<IActionResult> OnGetSetAvailable(int courierId)
    {
        if (_workContext.Admin == null)
            return RedirectToPage("managementLogin");
        using var _context = new LockerDbContext();
        var courier = await _context.Couriers.FirstOrDefaultAsync(x => x.Id == courierId);
        if (courier.IsAvailable)
            courier.IsAvailable = false;
        else
            courier.IsAvailable = true;

        await _context.SaveChangesAsync();
        if (!courier.IsAvailable)
        {
            var courierOrders =
                await _context.Orders.AnyAsync(x => x.CourierId == courierId && x.Status == (int)OrderStatus.Continue);
            if (courierOrders)
            {
                ViewData["message"] =
                    "Pasif yaptığınız kuryeye ait siparişler bulunuyor.Bu siparişlere kurye ataması yapın";
            }
        }
        Couriers = await _context.Couriers.Include(x => x.Region).ToListAsync();
        return Page();
    }
}