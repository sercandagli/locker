using Locker.Data;
using Locker.Entities;
using locker.Pages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Locker.Pages;



public class PartnerOrderListModel : BasePageModel
{

    private readonly WorkContext _workContext;
    public List<Order> Orders{get;set;}
    public PartnerOrderListModel(WorkContext workContext)
    {
        _workContext = workContext;
    }


    public async Task<IActionResult> OnGet(){
        
        if (_workContext.User == null)
            return RedirectToPage("login");
        using var _context = new LockerDbContext();

        Orders = await _context.Orders.Where(x => x.UserId == _workContext.User.Id).Include(x => x.OrderItems).ToListAsync();
        return Page();
    }
    
}