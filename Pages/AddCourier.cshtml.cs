using Locker;
using Locker.Data;
using Locker.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace locker.Pages;

public class AddCourierModel : BasePageModel
{
    private readonly WorkContext _workContext;

    public AddCourierModel(WorkContext workContext)
    {
        _workContext = workContext;
    }

    public List<Region> Regions { get; set; }

    public async Task<IActionResult> OnGet()
    {
        if (_workContext.Admin == null)
            return RedirectToPage("managementLogin");
        using var _context = new LockerDbContext();

        Regions = await _context.Regions.ToListAsync();
        return Page();
    }

    public async Task<IActionResult> OnPost(Courier courier)
    {
        if (_workContext.Admin == null)
            return RedirectToPage("managementLogin");

        using var _context = new LockerDbContext();
         if(!ModelState.IsValid){
                this.Message = "Lütfen tüm alanları doldurun";
                Regions = await _context.Regions.ToListAsync();

            return Page();

            }
        courier.Password = courier.Password.HashPassword();
        courier.IsAvailable = true;
        await _context.Couriers.AddAsync(courier);
        await _context.SaveChangesAsync();
        IsSuccess = true;
        Message = "Kurye Eklendi";
        return RedirectToPage("Couriers"); 

    }
}