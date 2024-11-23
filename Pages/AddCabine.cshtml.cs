using Locker;
using Locker.Data;
using Locker.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace locker.Pages;

public class AddCabineModel : BasePageModel
{
    private readonly WorkContext _workContext;

    public AddCabineModel(WorkContext workContext)
    {
        _workContext = workContext;
    }

    public List<Region> Regions { get; set; }
    public async Task<IActionResult> OnGet()
    {
        if (_workContext.Courier == null)
            return RedirectToPage("managementLogin");
        using var _context = new LockerDbContext();
        Regions = await _context.Regions.ToListAsync();
        return Page();
    }

    public async Task<IActionResult> OnPost(Cabine cabine)
    {
        if (_workContext.Courier == null)
            return RedirectToPage("managementLogin");
        using var _context = new LockerDbContext();
        cabine.ModifiedOn = DateTime.Now;
        cabine.IsActive = true;
        _context.Cabines.Add(cabine);
        await _context.SaveChangesAsync();
        return RedirectToPage("cabineList");
    }
}