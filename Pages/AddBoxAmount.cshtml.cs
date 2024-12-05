using Locker.Data;
using Locker.Entities;
using locker.Pages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Locker.Pages;

public class AddBoxAmountModel : BasePageModel
{

    private readonly WorkContext _workContext;
    public Box Box { get; set; }

    public AddBoxAmountModel(WorkContext workContext)
    {
        _workContext = workContext;
    }


    public async Task<IActionResult> OnGet(int boxId)
    {
        if (_workContext.Admin == null)
            return RedirectToPage("managementLogin");

        using var _context = new LockerDbContext();
        Box = await _context.Boxes.FirstOrDefaultAsync(x => x.Id == boxId);
        return Page();
    }

    public async Task<IActionResult> OnPost(BoxAmount boxAmount)
    {
        if (_workContext.Admin == null)
            return RedirectToPage("managementLogin");
        using var _context = new LockerDbContext();
        if(!ModelState.IsValid){
                this.Message = "Lütfen tüm alanları doldurun";
        Box = await _context.Boxes.FirstOrDefaultAsync(x => x.Id == boxAmount.BoxId);

            return Page();

            }

        try
        {
            var isAvailable = await _context.BoxAmounts.FirstOrDefaultAsync(x =>
                x.Id == boxAmount.BoxId && x.DeliveryType == boxAmount.DeliveryType &&
                x.UserType == boxAmount.UserType);
            if (isAvailable is not null)
            {
                isAvailable.Amount = boxAmount.Amount;
                isAvailable.ModifiedOn = DateTime.Now;
                isAvailable.PromotionAmount = boxAmount.PromotionAmount;
                isAvailable.PromotionDescription = boxAmount.PromotionDescription;
                isAvailable.IsPromotionActive = boxAmount.PromotionAmount != decimal.Zero;
            }
            else
            {
                await _context.BoxAmounts.AddAsync(new BoxAmount
                {
                    BoxId = boxAmount.BoxId,
                    UserType = boxAmount.UserType,
                    DeliveryType = boxAmount.DeliveryType,
                    Amount = boxAmount.Amount,
                    PromotionAmount = boxAmount.PromotionAmount,
                    PromotionDescription = boxAmount.PromotionDescription,
                    IsPromotionActive = boxAmount.PromotionAmount != null && boxAmount.PromotionAmount != decimal.Zero,
                    ModifiedOn = DateTime.Now
                });
            }

            await _context.SaveChangesAsync();
            ViewData["message"] = "İşlem başarılı";
        }
        catch (Exception ex)
        {
            ViewData["message"] = "Bir hata oluştu - " + ex.Message;
        }


        return RedirectToPage("BoxAmountList", new { boxId = boxAmount.BoxId });
    }
}