using Locker.Data;
using Locker.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Locker.Pages;



public class CancelOrderItemModel : PageModel {


    public CancelOrderItemModel(){

    }

    public async Task<IActionResult> OnGet(int orderItemId){

        using var _context = new LockerDbContext();
        var orderItem = await _context.OrderItems.FirstOrDefaultAsync(x => x.Id == orderItemId);

        var orderItemAmount = orderItem.Amount;

        var order = await _context.Orders.FirstOrDefaultAsync(x => x.Id == orderItem.OrderId);

        order.TotalAmount -= orderItemAmount;

        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == order.UserId);
        if (user.Type == (int)UserType.Commercial)
        {
            user.Credit += orderItemAmount;
            user.ModifiedOn = DateTime.Now;
        }

        orderItem.Status = (int)OrderItemStatus.Cancelled;

        await _context.SaveChangesAsync();

        return RedirectToPage("OrderDetail",new {id = orderItem.OrderId}); 

    }
}