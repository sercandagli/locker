using Locker.Data;
using Locker.Entities;
using locker.Pages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Locker.Pages;



public class BoxListModel : BasePageModel
{
    private readonly WorkContext _workContext;
    public List<Box> Boxes{get;set;}
    public BoxListModel(WorkContext workContext)
    {
        _workContext = workContext;
    }

    public async Task<IActionResult> OnGet(){
        
        if (_workContext.Admin == null)
            return RedirectToPage("managementLogin");
        using var _context = new LockerDbContext();

        Boxes = await _context.Boxes.Include(x => x.BoxAmounts).ToListAsync();

        return Page();
    }
}