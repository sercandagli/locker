using Locker.Data;
using Locker.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace locker.Pages;

public class OrderCompletedModel : BasePageModel
{
    public Order Order { get; set; }
    public async Task<IActionResult> OnGet()
    {
        var orderId = HttpContext.Session.GetInt32("orderId");
        if (orderId is null or 0)
            return RedirectToPage("lockerToLocker");

        using var _context = new LockerDbContext();

        Order = await _context.Orders.Include(x => x.OrderItems).FirstOrDefaultAsync(x => x.Id == orderId);
        return Page();
    }
}