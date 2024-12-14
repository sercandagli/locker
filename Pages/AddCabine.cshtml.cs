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
        if (_workContext.Admin == null)
            return RedirectToPage("managementLogin");
        using var _context = new LockerDbContext();
        Regions = await _context.Regions.ToListAsync();
        if (Regions.Count == 0)
        {
            this.Message = "Lütfen önce ana bölge ekleyin";
            
        }
        return Page();
    }

    public async Task<IActionResult> OnPost(Cabine cabine)
    {
        if (_workContext.Admin == null)
            return RedirectToPage("managementLogin");
            using var _context = new LockerDbContext();
            cabine.IsActive = true;
            if(!ModelState.IsValid){
                this.Message = "Lütfen tüm alanları doldurun";
                Regions = await _context.Regions.ToListAsync();

            return Page();

            }
        cabine.ModifiedOn = DateTime.Now;
        cabine.IsActive = true;
        _context.Cabines.Add(cabine);
        await _context.SaveChangesAsync();
        return RedirectToPage("cabineList");
    }
}