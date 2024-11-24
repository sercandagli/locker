using Locker.Data;
using Locker.Entities;
using locker.Pages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Locker.Pages;

public class EditBoxAmountModel : BasePageModel
{
    private readonly WorkContext _workContext;
    public BoxAmount BoxAmount { get; set; }

    public EditBoxAmountModel(WorkContext workContext)
    {
        _workContext = workContext;
    }


    public async Task<IActionResult> OnGet(int id)
    {
        if (_workContext.Admin == null)
            return RedirectToPage("managementLogin");
        using var _context = new LockerDbContext();
        BoxAmount = await _context.BoxAmounts.FirstOrDefaultAsync(x => x.Id == id);
        return Page();
    }

    public async Task<IActionResult> OnPost(BoxAmount boxAmount)
    {
        if (_workContext.Admin == null)
            return RedirectToPage("managementLogin");
        using var _context = new LockerDbContext();
        BoxAmount = await _context.BoxAmounts.FirstOrDefaultAsync(x => x.Id == boxAmount.Id);
        BoxAmount.Amount = boxAmount.Amount;
        BoxAmount.PromotionAmount = boxAmount.PromotionAmount;
        BoxAmount.PromotionDescription = boxAmount.PromotionDescription;
        BoxAmount.ModifiedOn = DateTime.Now;
        BoxAmount.IsPromotionActive = boxAmount.PromotionAmount != null && boxAmount.PromotionAmount != decimal.Zero;
        await _context.SaveChangesAsync();

        return RedirectToPage("BoxAmountList", new { boxId = BoxAmount.BoxId });
    }
}