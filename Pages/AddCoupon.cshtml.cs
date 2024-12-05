using locker.Pages;
using Locker.Data;
using Locker.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Locker.Pages;


public class AddCouponModel : BasePageModel{

private readonly WorkContext _workContext;
public AddCouponModel(WorkContext workContext){
    _workContext = workContext;
}
    public async Task<IActionResult> OnGet(){
        if (_workContext.Admin == null)
            return RedirectToPage("managementLogin");
            return Page();
    }

    public async Task<IActionResult> OnPost(Coupon coupon){



if (_workContext.Admin == null)
            return RedirectToPage("managementLogin");

            using var _context = new LockerDbContext();
    var isCodeActive = await _context.Coupons.FirstOrDefaultAsync(x => !x.IsUsed && x.Code == coupon.Code);
    if(isCodeActive != null){
        this.Message = "Bu kod ile bir kupon tanımlı";
                    return Page();

    }
           await _context.Coupons.AddAsync(new Coupon{
                Amount = coupon.Amount,
                IsUsed = false,
                Code = coupon.Code
           });

        await _context.SaveChangesAsync();

        
        return RedirectToPage("couponList");
    }
    
}