using locker.Pages;
using Locker.Data;
using Locker.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Locker.Pages;


public class CouponListModel : BasePageModel{
    private readonly WorkContext _workContext;
    public List<Coupon> Coupons{get;set;}


    public CouponListModel(WorkContext workContext){
        _workContext = workContext;
    }


    public async Task<IActionResult> OnGet(){
          if (_workContext.Admin == null)
            return RedirectToPage("managementLogin");
using var _context = new LockerDbContext();
        Coupons = await _context.Coupons.ToListAsync();           

            return Page();

            
    }

    public async Task<IActionResult> OnGetDeleteCoupon(int id){
         if (_workContext.Admin == null)
            return RedirectToPage("managementLogin");
                using var _context = new LockerDbContext();
         
         _context.Remove(_context.Coupons.FirstOrDefault(x => x.Id == id));
         await _context.SaveChangesAsync();
        Coupons = await _context.Coupons.Where(x => !x.IsUsed).ToListAsync();           

            return Page();
    }
}