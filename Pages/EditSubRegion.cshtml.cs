using Locker;
using Locker.Data;
using Locker.Entities;
using Microsoft.AspNetCore.DataProtection.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace locker.Pages;

public class EditRegionModel : BasePageModel
{
    private readonly WorkContext _workContext;

    public EditRegionModel(WorkContext workContext)
    {
        _workContext = workContext;
    }

    public List<Region> Regions { get; set; }
    public SubRegion SubRegion { get; set; }

    public async Task<IActionResult> OnGet(int id)
    {
        if (_workContext.Admin == null)
            return RedirectToPage("managementLogin");
        using var _context = new LockerDbContext();
        Regions = await _context.Regions.ToListAsync();
        SubRegion = await _context.SubRegions.FirstOrDefaultAsync(x => x.Id == id);
        return Page();
    }

    public async Task<IActionResult> OnPost(SubRegion model)
    {
        if (_workContext.Admin == null)
            return RedirectToPage("managementLogin");
        using var _context = new LockerDbContext();
        var region = await _context.SubRegions.FirstOrDefaultAsync(x => x.Id == model.Id);
        region.Name = model.Name;
        region.RegionId = model.RegionId;
        region.Coordinates = model.Coordinates;
        region.ModifiedOn = DateTime.Now;
        await _context.SaveChangesAsync();
        return RedirectToPage("regionList");
    }
}