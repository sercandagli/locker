using Locker;
using Locker.Data;
using Locker.Entities;
using Locker.Enums;
using Locker.Models;
using Locker.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace locker.Pages;

public class AddressToAddressModel(IOrderService orderService, WorkContext workContext) : BasePageModel
{
    private readonly IOrderService _orderService = orderService;
    private readonly WorkContext _workContext = workContext;
    public required OrderViewModel Model { get; set; }
    public required List<SubRegion> SubRegions { get; set; }

    public async Task<IActionResult> OnGet()
    {
        if (_workContext.User == null)
            return RedirectToPage("lockerToLocker");
        
        if(_workContext.User.Type != (int)UserType.Partner)
            return RedirectToPage("notFound");
        
        var userType = _workContext.User?.Type ?? 1;
        Model = await _orderService.PrepareOrderPage((int)DeliveryType.AddressToAddress, userType);
        using var _context = new LockerDbContext();

        SubRegions = await _context.SubRegions.Where(x => x.IsActive).ToListAsync();
        return Page();
    }
    
    public async Task<IActionResult> OnPost(CreateOrderViewModel model)
    {
        model.DeliveryType = DeliveryType.AddressToAddress;
        model.OrderType = UserType.Partner;
        await _orderService.CreateOrder(model);
        return RedirectToPage("Payment");
    }
}