using Locker;
using Locker.Data;
using Locker.Entities;

namespace locker.Pages;

public class CabinePointsModel : BasePageModel
{

    public List<Cabine> Cabines { get; set; } = new List<Cabine>();

    public async Task OnGet()
    {
        using var _context = new LockerDbContext();
        Cabines = _context.Cabines.ToList();
    }
        
    
}