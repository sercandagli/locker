using Locker;
using Locker.Data;
using Locker.Entities;
using Locker.Enums;
using Locker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace locker.Pages;

public class PaymentModel : BasePageModel
{
    public Order Order { get; set; }
    private readonly WorkContext _workContext;
    public User User { get; set; }
    
    public string CardNumber { get; set; }
    public string ExpiryMonth { get; set; }
    public string ExpiryYear { get; set; }
    public string Cvv { get; set; }
    
    public bool PayWithCredit { get; set; }


    public PaymentModel(WorkContext workContext)
    {
        _workContext = workContext;
    }
    
    public async Task<IActionResult> OnGet()
    {
        var orderId = HttpContext.Session.GetInt32("orderId");
        if (orderId is null or 0)
            return RedirectToPage("lockerToLocker");

        using var _context = new LockerDbContext();

        Order = await _context.Orders.Include(x => x.OrderItems).FirstOrDefaultAsync(x => x.Id == orderId);

        if (_workContext.User is { Type: (int)UserType.Partner })
        {
            Order.IsPaid = true;
            Order.PayType = (int)PayType.Transfer;
            Order.PaidDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return RedirectToPage("OrderCompleted");
        }


        if (_workContext.User is not null)
            User = await _context.Users.FirstOrDefaultAsync(x => x.Id == _workContext.User.Id);
        return Page();
    }



    public async Task<IActionResult> OnPostApplyCoupon(CheckCouponModel model){
        var orderId = HttpContext.Session.GetInt32("orderId");
            if (orderId is null or 0)
                return RedirectToPage("lockerToLocker");

                    using var _context = new LockerDbContext();
                var coupon = await _context.Coupons.FirstOrDefaultAsync(x => x.Code == model.Code && !x.IsUsed);
                if(coupon == null){
                    return new JsonResult(new {
                        Message = "Geçersiz kod"
                    });
                }

                var order = await _context.Orders.FirstOrDefaultAsync(x => x.Id == orderId);
                order.TotalAmount -= order.TotalAmount > coupon.Amount ? coupon.Amount : order.TotalAmount;
                order.Discount = order.TotalAmount == decimal.Zero ? order.TotalAmount : coupon.Amount;
                order.ModifiedOn = DateTime.Now;
                coupon.IsUsed = true;
                await  _context.SaveChangesAsync();

                return new JsonResult(new {
                    isSuccess = true,
                    newOrderAmount = order.TotalAmount,
                    couponId = coupon.Id,
                    discount = order.Discount
                });

    }


    public async Task<IActionResult> OnPostRevertCoupon(CheckCouponModel model){
        var orderId = HttpContext.Session.GetInt32("orderId");
            if (orderId is null or 0)
                return RedirectToPage("lockerToLocker");

                    using var _context = new LockerDbContext();
                var coupon = await _context.Coupons.FirstOrDefaultAsync(x => x.Id == model.CouponId && x.IsUsed);
                if(coupon == null){
                    return new JsonResult(new {
                        Message = "İşlem Başarısız"
                    });
                }

                var order = await _context.Orders.FirstOrDefaultAsync(x => x.Id == orderId);
                order.TotalAmount += order.Discount;
                order.Discount = 0;
                order.ModifiedOn = DateTime.Now;
                coupon.IsUsed = false;
                await  _context.SaveChangesAsync();

                return new JsonResult(new {
                    isSuccess = true,
                    newOrderAmount = order.TotalAmount,
                    couponId = 0,
                    discount = 0
                });

    }













    public async Task<IActionResult> OnPost(PaymentPostModel model)
    {
        var orderId = HttpContext.Session.GetInt32("orderId");
        if (orderId is null or 0)
            return RedirectToPage("lockerToLocker");

        using var _context = new LockerDbContext();

        var order = await _context.Orders.FirstOrDefaultAsync(x => x.Id == orderId);
        var user = _workContext.User ?? await _context.Users.FirstOrDefaultAsync(x => x.Id == order.UserId);
        var total = order.TotalAmount;
        if (model.PayWithCredit)
        {
            if (user.Type != (int)UserType.Partner)
                return RedirectToPage("lockerToLocker");
            var totalCredit = user.Credit + user.PromotionCredit;
            if (totalCredit < total)
                return RedirectToPage("lockerToLocker");
            order.IsPaid = true;
            order.PaidDate = DateTime.Now;
            order.PayType = (int)PayType.Credit;
            if (user.PromotionCredit >= totalCredit)
            {
                user.PromotionCredit = 0;
            }
            else
            {
                total -= user.PromotionCredit;
                user.PromotionCredit = 0;
                user.Credit -= total;
            }

            user.ModifiedOn = DateTime.Now;

        }
        else
        {
            order.PayType = (int)PayType.CreditCard;
        }
          order.IsPaid = true;
          order.PaidDate = DateTime.Now;

          await _context.SaveChangesAsync();
          return RedirectToPage("orderCompleted");
    }
}