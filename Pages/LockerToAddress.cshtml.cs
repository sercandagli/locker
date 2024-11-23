using Locker.Data;
using Locker.Entities;
using Locker.Enums;
using Locker.Models;
using locker.Pages;
using Locker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Locker.Pages;

public class LockerToAddressModel(IOrderService orderService, WorkContext workContext) : BasePageModel
{
    private readonly IOrderService _orderService = orderService;
    private readonly WorkContext _workContext = workContext;
    public required OrderViewModel Model { get; set; }
    public List<SubRegion> SubRegions { get; set; }

    public async Task OnGet()
    {
        var userType = _workContext.User?.Type ?? 1;
        Model = await _orderService.PrepareOrderPage((int)DeliveryType.LockerToAddress, userType);
        using var _context = new LockerDbContext();
        SubRegions = await _context.SubRegions.Where(x => x.IsActive).ToListAsync();
        User = _workContext.User;
    }


    public async Task<IActionResult> OnPost(CreateOrderViewModel model)
    {
        model.DeliveryType = DeliveryType.LockerToAddress;
        model.OrderType = _workContext.User != null ? (UserType)_workContext.User.Type : UserType.Normal;
        await _orderService.CreateOrder(model);
        return RedirectToPage("Payment");

    }
}