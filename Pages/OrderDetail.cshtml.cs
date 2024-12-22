using Locker.Data;
using Locker.Entities;
using Locker.Enums;
using Locker.Models;
using locker.Pages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Locker.Pages;


public class OrderDetailModel : BasePageModel
{

    private readonly WorkContext _workContext;

    public OrderDetailModel(WorkContext workContext)
    {
        _workContext = workContext;
    }

    public Order OrderDetail{get;set;}
    public async Task<IActionResult> OnGet(int id){
        if (_workContext.Admin == null)
            return RedirectToPage("managementLogin");
        using var _context = new LockerDbContext();
        OrderDetail = await _context.Orders.Include(x => x.OrderItems).FirstOrDefaultAsync(x => x.Id == id);
        return Page();
    }

    public async Task<IActionResult> OnPostChangeStatus(ChangeStatusModel model)
    {
        if (_workContext.Admin == null)
            return RedirectToPage("managementLogin");
        using var _context = new LockerDbContext();
        var orderItem = await _context.OrderItems.FirstOrDefaultAsync(x => x.Id == model.OrderItemId);
        if (orderItem == null)
        {
            return new JsonResult(new
            {
                Message = "Sipariş bulunamadı"
            });
        }
        var order = await _context.Orders.Include(x => x.OrderItems).FirstOrDefaultAsync(x => x.Id == orderItem.OrderId);
        
        orderItem.Status = model.Status;
        if (model.Status == (int)OrderItemStatus.Delivered)
        {
            if (order.OrderItems.Where(x => x.Id != model.OrderItemId)
                .All(x => x.Status == (int)OrderItemStatus.Delivered))
            {
                order.Status = (int)OrderStatus.Completed;
            }
        }
        orderItem.ModifiedOn = DateTime.Now;
        await _context.SaveChangesAsync();

        return new JsonResult(new
        {
            isSuccess = true
        });
    }

    public async Task OnPostCancelOrderItem(AdminOrderItemCancelModel model)
    {
        if (_workContext.Admin == null)
            return;
        using var _context = new LockerDbContext();

        var orderItem = await _context.OrderItems.FirstOrDefaultAsync(x => x.Id == model.OrderItemId);

        var orderItemAmount = orderItem.Amount;

        var order = await _context.Orders.FirstOrDefaultAsync(x => x.Id == orderItem.OrderId);
        var allOrderItems = await _context.OrderItems.Where(x => x.OrderId == order.Id).ToListAsync();
        order.TotalAmount -= orderItemAmount;

        if(allOrderItems.Where(x => x.Id != orderItem.Id).All(x => x.Status == (int)OrderItemStatus.Cancelled)){
            order.Status = (int)OrderStatus.Cancelled;
        }else if(allOrderItems.Where(x => x.Id != orderItem.Id).All(x => x.Status == (int)OrderItemStatus.Delivered))
        {
            order.Status = (int)OrderStatus.Completed;
        }
        orderItem.Status = (int)OrderItemStatus.Cancelled;
        orderItem.Amount = decimal.Zero;

        await _context.SaveChangesAsync();
        
    }

    public async Task<IActionResult> OnPostSendOtp(SendOtpModel model)
    {
        if (_workContext.Admin == null)
            return new JsonResult(new {});
        using var _context = new LockerDbContext();
        var order = await _context.Orders.Include(x => x.OrderItems).FirstOrDefaultAsync(x => x.Id == model.OrderId);
        if (order == null)
            return new JsonResult(new { Message = "Sipariş Bulunamadı" });

        if (model.SendSender)
        {
            if (order.DeliveryType is (int)DeliveryType.LockerToAddress or (int)DeliveryType.LockerToLocker)
            {
                foreach (var orderItem in order.OrderItems)
                {
                    SendOtp(order.SenderPhone,orderItem.SLSaveCde);
                }
            }else if (order.DeliveryType is (int)DeliveryType.AddressToAddress or (int)DeliveryType.AddressToLocker)
            {
                
                    SendOtp(order.SenderPhone,order.SenderOtpCode);
                
            } 
        }
        
        if (model.SendReceiver)
        {
            if (order.DeliveryType is (int)DeliveryType.LockerToAddress or (int)DeliveryType.AddressToAddress)
            {
                
                  SendOtp(order.ReceiverPhone,order.SenderOtpCode);
                
            }else if (order.DeliveryType is (int)DeliveryType.AddressToLocker or (int)DeliveryType.LockerToLocker)
            {
                foreach (var orderItem in order.OrderItems)
                {
                    SendOtp(order.ReceiverPhone,orderItem.TLPickCode);
                }
            } 
        }
        
        return new JsonResult(new { IsSuccess=true });


    }

    


    private void SendOtp(string phone,string code)
    {
        
    }


}