using Locker;
using Locker.Data;
using Locker.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace locker.Pages;

public class RegionListModel : BasePageModel
{
    private readonly WorkContext _workContext;

    public RegionListModel(WorkContext workContext)
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
}