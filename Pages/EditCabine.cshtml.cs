using Locker;
using Locker.Data;
using Locker.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace locker.Pages;

public class EditCabineModel : BasePageModel
{
    private readonly WorkContext _workContext;

    public EditCabineModel(WorkContext workContext)
    {
        _workContext = workContext;
    }

    public Cabine Cabine { get; set; }
    public List<Region> Regions { get; set; }
    public async Task<IActionResult> OnGet(int id)
    {
        if (_workContext.Admin == null)
            return RedirectToPage("managementLogin");
        using var _context = new LockerDbContext();
        Cabine = await _context.Cabines.FirstOrDefaultAsync(x => x.Id == id);
        Regions = await _context.Regions.ToListAsync();
        return Page();
    }
    
    
    public async Task<IActionResult> OnPost(Cabine model)
    {
        if (_workContext.Admin == null)
            return RedirectToPage("managementLogin");
        using var _context = new LockerDbContext();
        Cabine = await _context.Cabines.FirstOrDefaultAsync(x => x.Id == model.Id);
        Cabine.Name = model.Name;
        Cabine.Lat = model.Lat;
        Cabine.Long = model.Long;
        Cabine.Identifier = model.Identifier;
        Cabine.ModifiedOn = DateTime.Now;
        Cabine.Description = model.Description;
        Cabine.RegionId = model.RegionId;
        await _context.SaveChangesAsync();
        return RedirectToPage("cabineList");
    }
    
}