using Locker.Data;
using Locker.Entities;
using Microsoft.EntityFrameworkCore;

namespace locker.Pages;

public class RegionDetailModel : BasePageModel
{
    public List<SubRegion> SubRegions { get; set; }

    public async Task OnGet(int id)
    {
        using var _context = new LockerDbContext();
        SubRegions = await _context.SubRegions.Where(x => x.RegionId == id).ToListAsync();
    }
}