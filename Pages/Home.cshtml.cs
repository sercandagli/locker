using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace locker.Pages;

public class HomeModel : BasePageModel
{
    private readonly ILogger<HomeModel> _logger;

    public HomeModel(ILogger<HomeModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {

    }
}
