using Locker.Entities;
using Locker.Enums;
using Locker.Models;
using locker.Pages;
using Locker.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Locker.Pages;

public class LockerToLockerModel(IOrderService orderService, WorkContext workContext) : BasePageModel
{
    private readonly IOrderService _orderService = orderService;
    private readonly WorkContext _workContext = workContext;
    public required OrderViewModel Model { get; set; }

    public async Task OnGet()
    {
        var userType = _workContext.User?.Type ?? 1;
        Model = await _orderService.PrepareOrderPage((int)DeliveryType.LockerToLocker, userType);
        User = _workContext.User;
    }

    public async Task<IActionResult> OnPost(CreateOrderViewModel model)
    {
        model.DeliveryType = DeliveryType.LockerToLocker;
        model.OrderType = _workContext.User != null ? (UserType)_workContext.User.Type : UserType.Normal;
        await _orderService.CreateOrder(model);
        return RedirectToPage("Payment");
    }
}