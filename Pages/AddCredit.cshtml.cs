using Locker;
using Locker.Data;
using Locker.Entities;
using Locker.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace locker.Pages;

public class AddCreditModel : BasePageModel
{
    private readonly WorkContext _workContext;

    public AddCreditModel(WorkContext workContext)
    {
        _workContext = workContext;
    }

    public User User { get; set; }

    public async Task<IActionResult> OnGet(int userId)
    {
        if (_workContext.Admin == null)
            return RedirectToPage("managementLogin");
        using var _context = new LockerDbContext();

        User = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);

        if (User.Type != (int)UserType.Commercial)
            return RedirectToPage("userList");
        return Page();
    }
    
    public async Task<IActionResult> OnPost(User user)
    {
        if (_workContext.Admin == null)
            return RedirectToPage("managementLogin");
        using var _context = new LockerDbContext();

        User = await _context.Users.FirstOrDefaultAsync(x => x.Id == user.Id);
        User.Credit += user.Credit;
        User.PromotionCredit += user.PromotionCredit;
        User.ModifiedOn = DateTime.Now;
        await _context.SaveChangesAsync();
        return RedirectToPage("userList");

    }
    
}