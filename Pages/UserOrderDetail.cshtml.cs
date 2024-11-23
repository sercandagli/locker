using Locker;
using Locker.Data;
using Locker.Entities;
using Locker.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace locker.Pages;

public class UserOrderDetailModel : BasePageModel
{    
    private WorkContext _workContext { get; set; }

    public UserOrderDetailModel(WorkContext workContext)
    {
        _workContext = workContext;
    }

    public Order Order { get; set; }

    public async Task OnGet(int orderId)
    {
        using var _context = new LockerDbContext();
        Order = await _context.Orders.Include(x => x.OrderItems).FirstOrDefaultAsync(x => x.Id == orderId && x.UserId == _workContext.User.Id);
        
    }

    public async Task<IActionResult> OnGetCancel(int orderItemId)
    {
        using var _context = new LockerDbContext();
        var orderItem = await _context.OrderItems.Include(x => x.Order).FirstOrDefaultAsync(x => x.Id == orderItemId);
        if(orderItem.Order.UserId != _workContext.User.Id)
            return RedirectToPage("home");

        var orderItemAmount = orderItem.Amount;
        orderItem.Status = (int)OrderItemStatus.Cancelled;
        orderItem.ModifiedOn = DateTime.Now;
        orderItem.Order.TotalAmount -= orderItem.Amount;
        orderItem.Amount = decimal.Zero;
        orderItem.Order.ModifiedOn = DateTime.Now;

        if (_workContext.User.Type == (int)UserType.Commercial)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == _workContext.User.Id);
            user.Credit += orderItemAmount;
            user.ModifiedOn = DateTime.Now;
        }

        var orderItems = await _context.OrderItems.Where(x => x.OrderId == orderItem.Order.Id).ToListAsync();
        if (orderItems.Where(x => x.Id != orderItemId).All(x => x.Status == (int)OrderItemStatus.Cancelled))
            orderItem.Order.Status = (int)OrderStatus.Cancelled;

        await _context.SaveChangesAsync();

        return RedirectToPage("userOrders");


    }
}