using Locker;
using Locker.Data;
using Locker.Entities;
using Locker.Models;
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

    public async Task<IActionResult> OnPost(AddCabineViewModel cabine)
    {
        if (_workContext.Admin == null)
            return RedirectToPage("managementLogin");
        using var _context = new LockerDbContext();

        var newCabine = new Cabine()
        {
            RegionId = cabine.RegionId,
            Name = cabine.Name,
            Description = cabine.Description,
            Lat = cabine.Lat,
            Long = cabine.Long,
            Identifier = cabine.Identifier,
            IsActive = true,
            ModifiedOn = DateTime.Now
        };

        if (cabine.File != null)
        {
            var extent = Path.GetExtension(cabine.File.FileName);
            var randomName = ($"{Guid.NewGuid()}{extent}");
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/assets/localImages", randomName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await cabine.File.CopyToAsync(stream);
            }
            newCabine.ImagePath = "~/assets/localImages/" + randomName;

        }

        _context.Cabines.Add(newCabine);
        await _context.SaveChangesAsync();
        return RedirectToPage("cabineList");
    }
}