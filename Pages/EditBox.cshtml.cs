using Locker.Data;
using Locker.Entities;
using locker.Pages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
namespace Locker.Pages;



public class EditBoxModel : BasePageModel{
    private readonly WorkContext _workContext;
    public Box Box{get;set;}
    public  EditBoxModel(WorkContext workContext)
    {
        _workContext = workContext;
    }


    public async Task<IActionResult> OnGet(int id){
        if (_workContext.Admin == null)
            return RedirectToPage("managementLogin");
        using var _context = new LockerDbContext();
        Box = await _context.Boxes.FirstOrDefaultAsync(x => x.Id == id);
        return Page();

    }

    public async Task<IActionResult> OnPost(Box box){
        if (_workContext.Admin == null)
            return RedirectToPage("managementLogin");
        using var _context = new LockerDbContext();

        Box = await _context.Boxes.FirstOrDefaultAsync(x => x.Id == box.Id);
        if(Box is null)
        return RedirectToPage("BoxList"); 
        Box.Size = box.Size;
        Box.IconLink = box.IconLink;
        await _context.SaveChangesAsync();
        return RedirectToPage("BoxList"); 

    }
}