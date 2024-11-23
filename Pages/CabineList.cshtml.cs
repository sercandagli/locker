using Locker;
using Locker.Data;
using Locker.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace locker.Pages;

public class CabineListModel : BasePageModel
{
    private readonly WorkContext _workContext;

    public CabineListModel(WorkContext workContext)
    {
        _workContext = workContext;
    }

    public List<Cabine> Cabines { get; set; }

    public async Task<IActionResult> OnGet()
    {
        if (_workContext.Admin == null)
            return RedirectToPage("managementLogin");
        using var _context = new LockerDbContext();
        Cabines = await _context.Cabines.Include(x => x.Region).ToListAsync();
        return Page();
    }

    public async Task<IActionResult> OnGetSetCabineStatus(int id)
    {
        if (_workContext.Admin == null)
            return RedirectToPage("managementLogin");
        using var _context = new LockerDbContext();
        var cabine = await _context.Cabines.FirstOrDefaultAsync(x => x.Id == id);
        cabine.IsActive = !cabine.IsActive;
        cabine.ModifiedOn = DateTime.Now;
        await _context.SaveChangesAsync();
        return RedirectToPage("cabineList");
    }
}