using Locker;
using Locker.Data;
using Locker.Entities;
using Locker.Enums;
using Locker.Models;
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

    public AddCreditViewModel CreditModel { get; set; }

    public async Task<IActionResult> OnGet(int userId)
    {
        if (_workContext.Admin == null)
            return RedirectToPage("managementLogin");
        using var _context = new LockerDbContext();

        User = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);

        if (User.Type != (int)UserType.Commercial)
            return RedirectToPage("userList");

            CreditModel = new AddCreditViewModel{
                UserId = User.Id
            };
        return Page();
    }
    
    public async Task<IActionResult> OnPost(AddCreditViewModel model)
    {
        if (_workContext.Admin == null)
            return RedirectToPage("managementLogin");
        using var _context = new LockerDbContext();
 if(!ModelState.IsValid){
                this.Message = "Lütfen tüm alanları doldurun";
  User = await _context.Users.FirstOrDefaultAsync(x => x.Id == model.UserId);
CreditModel = new AddCreditViewModel();
            return Page();

            }
        User = await _context.Users.FirstOrDefaultAsync(x => x.Id == model.UserId);
        User.Credit += model.Credit;
        User.PromotionCredit += model.PromotionCredit ?? decimal.Zero;
        User.ModifiedOn = DateTime.Now;
        await _context.SaveChangesAsync();
        return RedirectToPage("userList");

    }
    
}