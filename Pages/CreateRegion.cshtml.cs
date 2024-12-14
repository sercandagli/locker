using System.Text.Json;
using Locker;
using Locker.Data;
using Locker.Entities;
using Locker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace locker.Pages;

public class CreateRegionModel : BasePageModel
{
    private readonly WorkContext _workContext;

    public CreateRegionModel(WorkContext workContext)
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

    public async Task<IActionResult> OnPost(SubRegion model)
    {
        if (_workContext.Admin == null)
            return RedirectToPage("managementLogin");
        using var _context = new LockerDbContext();
        
        model.ModifiedOn = DateTime.Now;
        model.IsActive = true;
        _context.SubRegions.Add(model);
        
        await _context.SaveChangesAsync();
        return RedirectToPage("regionList");
    }
}