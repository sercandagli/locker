using Locker.Data;
using Locker.Entities;
using locker.Pages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Locker.Pages;

public class AboutUsModel : BasePageModel
{

    private readonly WorkContext _workContext;
    public Box Box { get; set; }

    public AboutUsModel(WorkContext workContext)
    {
        _workContext = workContext;
    }

    public async Task<IActionResult> OnGet()
    {
        return Page();
    }
}