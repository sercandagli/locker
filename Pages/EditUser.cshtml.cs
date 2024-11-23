using Locker;
using Locker.Data;
using Locker.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace locker.Pages;

public class EditUserModel : BasePageModel
{
    private readonly WorkContext _workContext;

    public EditUserModel(WorkContext workContext)
    {
        _workContext = workContext;
    }

    public User User { get; set; }
    public async Task<IActionResult> OnGet(int id)
    {
        if (_workContext.Admin == null)
            return RedirectToPage("managementLogin");
        using var _context = new LockerDbContext();
        User = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
        return Page();
    }
    
    
    public async Task<IActionResult> OnPost(User model)
    {
        if (_workContext.Admin == null)
            return RedirectToPage("managementLogin");
        using var _context = new LockerDbContext();
        User = await _context.Users.FirstOrDefaultAsync(x => x.Id == model.Id);

        User.Name = model.Name;
        User.Email = model.Email;
        User.Phone = model.Phone;
        User.Address = model.Address;
        User.AddressLat = model.AddressLat;
        User.AddressLong = model.AddressLong;
        User.ModifiedOn = DateTime.Now;
        await _context.SaveChangesAsync();
        return RedirectToPage("userList");
    }
}