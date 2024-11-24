using Locker;
using Locker.Data;
using Locker.Models;
using Locker.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace locker.Pages;



public class LogoutModel(WorkContext workContext) : BasePageModel
{
    private readonly WorkContext _workContext = workContext;

  
    public async Task<IActionResult> OnGet()
    {
        _workContext.User = null;
        HttpContext.Session.SetInt32("IsLoggedIn",0);
        HttpContext.Session.SetString("Name","");
        HttpContext.Session.SetInt32("UserId",0);
        HttpContext.Session.SetInt32("Role", 0);
        return RedirectToPage("index");
    }


   
    
}