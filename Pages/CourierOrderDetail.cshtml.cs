using Locker;
using Locker.Data;
using Locker.Entities;
using Locker.Enums;
using Locker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace locker.Pages;

public class CourierOrderDetailModel : BasePageModel
{
    public Order Order { get; set; }
    private readonly WorkContext _workContext;

    public CourierOrderDetailModel(WorkContext workContext)
    {
        _workContext = workContext;
    }


    public async Task<IActionResult> OnGet(int id)
    {
        if (_workContext.Courier == null)
            return RedirectToPage("courierLogin");
        
        using var _context = new LockerDbContext();
        Order = await _context.Orders.Include(x => x.OrderItems).FirstOrDefaultAsync(x => x.Id == id);
        return Page();
    }

      public async Task<IActionResult> OnPostTakeDelivery(SetDeliveryModel model)
    {
        if (_workContext.Courier == null)
            return RedirectToPage("courierLogin");
        
        using var _context = new LockerDbContext();
        var order = await _context.Orders.FirstOrDefaultAsync(x => x.Id == model.OrderId);
        if (order == null)
            return
                new JsonResult(new
                {
                    Message = "Teslim Alınacak Sipariş Bulunamadı",
                    IsSuccess = false,
                });

        if (!string.IsNullOrEmpty(order.SenderOtpCode) && order.SenderOtpCode != model.OTPCode)
            return new JsonResult(new

            {
                Message = "OTP Kodu Yanlış",
                IsSuccess = false,
            });
        var orderItems = await _context.OrderItems.Where(x => x.OrderId == model.OrderId).ToListAsync();

        foreach (var orderItem in orderItems)
        {
            orderItem.Status = (int)OrderItemStatus.AtCourier;
            orderItem.ModifiedOn = DateTime.Now;
        }
      
        await _context.SaveChangesAsync();

    

        return new JsonResult(new
        {
            IsSuccess = true
        });
    }


    public async Task<IActionResult> OnPostSetDelivery(SetDeliveryModel model)
    {
        if (_workContext.Courier == null)
            return RedirectToPage("courierLogin");
        
        using var _context = new LockerDbContext();
        var order = await _context.Orders.FirstOrDefaultAsync(x => x.Id == model.OrderId);
        if (order == null)
            return
                new JsonResult(new
                {
                    Message = "Teslim Edilecek Sipariş Bulunamadı",
                    IsSuccess = false,
                });

        var allOrderItems = await _context.OrderItems.Where(x => x.OrderId == model.OrderId).ToListAsync();
     
        if (!string.IsNullOrEmpty(order.ReceiverOtpCode) && order.ReceiverOtpCode != model.OTPCode)
            return new JsonResult(new

            {
                Message = "OTP Kodu Yanlış",
                IsSuccess = false,
            });
        foreach (var orderItem in allOrderItems)
        {
            orderItem.Status = (int)OrderItemStatus.Delivered;
            orderItem.ModifiedOn = DateTime.Now; 
        }
   
        await _context.SaveChangesAsync();

        if (allOrderItems.Any(x => x.Status != (int)OrderItemStatus.Delivered))
            return new JsonResult(new
            {
                IsSuccess = true
            });


        order.Status = (int)OrderStatus.Completed;
        order.ModifiedOn = DateTime.Now;


        return new JsonResult(new
        {
            IsSuccess = true
        });
    }
}