using Locker;
using Locker.Data;
using Locker.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace locker.Pages;

public class EditCourierModel : BasePageModel
{
    private readonly WorkContext _workContext;

    public EditCourierModel(WorkContext workContext)
    {
        _workContext = workContext;
    }

    public Courier Courier { get; set; }
    public List<Region> Regions { get; set; }
    public async Task<IActionResult> OnGet(int id)
    {
        if (_workContext.Admin == null)
            return RedirectToPage("managementLogin");
        using var _context = new LockerDbContext();

        Courier = await _context.Couriers.FirstOrDefaultAsync(x => x.Id == id);
        Regions = await _context.Regions.ToListAsync();

        return Page();
    }

    public async Task<IActionResult> OnPost(Courier model){
          if (_workContext.Admin == null)
            return RedirectToPage("managementLogin");
        using var _context = new LockerDbContext();

        Courier = await _context.Couriers.FirstOrDefaultAsync(x => x.Id == model.Id);
        Regions = await _context.Regions.ToListAsync();

        Courier.Name = model.Name;
        Courier.Email = model.Email;
        Courier.RegionId = model.RegionId;
        if(string.IsNullOrEmpty(model.Password))
        Courier.Password = model.Password.HashPassword();
        await _context.SaveChangesAsync();

        return RedirectToPage("couriers");
    }
}