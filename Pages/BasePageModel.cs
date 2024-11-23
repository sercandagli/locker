using Locker.Entities;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace locker.Pages;

public class BasePageModel : PageModel
{
    public string? Message { get; set; }

    public bool IsSuccess { get; set; }
    
    public User User { get; set; }
    
    public Courier Courier { get; set; }
    
    public bool IsCourierLoggedIn { get; set; }
    
    public bool IsUserLoggedIn { get; set; }
    
    public bool IsAdminLoggedIn { get; set; }
}