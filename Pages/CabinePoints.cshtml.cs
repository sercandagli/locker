using Locker;
using Locker.Data;
using Locker.Entities;
using Microsoft.EntityFrameworkCore;

namespace locker.Pages;

public class CabinePointsModel : BasePageModel
{

    public List<Cabine> Cabines { get; set; } = new List<Cabine>();

    public async Task OnGet()
    {
        using var _context = new LockerDbContext();
        Cabines = await _context.Cabines.Where(x => x.IsActive).ToListAsync();
    }
        
    
}