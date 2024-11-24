using Locker.Data;
using Locker.Entities;
using locker.Pages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Locker.Pages;

public class AddBoxModel : BasePageModel
{

    private readonly WorkContext _workContext;
    public Box Box { get; set; }

    public AddBoxModel(WorkContext workContext)
    {
        _workContext = workContext;
    }


    public async Task<IActionResult> OnGet()
    {
        if (_workContext.Admin == null)
            return RedirectToPage("managementLogin");


        return Page();
    }

    public async Task<IActionResult> OnPost(Box box)
    {
        if (_workContext.Admin == null)
            return RedirectToPage("managementLogin");
        using var _context = new LockerDbContext();

         _context.Boxes.Add(new Box{
            Size = box.Size,
            IconLink = box.IconLink

        });

        await _context.SaveChangesAsync();
        return RedirectToPage("boxList");
    }
}