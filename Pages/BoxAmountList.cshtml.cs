using Locker.Data;
using Locker.Entities;
using locker.Pages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Locker.Pages;



public class BoxAmountListModel : BasePageModel{
    private readonly WorkContext _workContext;

    public BoxAmountListModel(WorkContext workContext)
    {
        _workContext = workContext;
    }

    public List<BoxAmount> BoxAmounts{get;set;}
    public int BoxId{get;set;}



    public async Task<IActionResult> OnGet(int boxId){
        if (_workContext.Admin == null)
            return RedirectToPage("managementLogin");
        BoxId = boxId;
        using var _context = new LockerDbContext();
        BoxAmounts = await _context.BoxAmounts.Include(x => x.Box).Where(x =>x.BoxId == boxId).ToListAsync();
        return Page();
    }
}