using Locker.Data;
using Locker.Entities;
using locker.Pages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Locker.Pages;


public class UserListModel : BasePageModel
{

    private readonly WorkContext _workContext;

    public UserListModel(WorkContext workContext)
    {
        _workContext = workContext;
    }

    public List<User> Users{get;set;}


    public async Task<IActionResult> OnGet(int page = 1,int type = 0,string name=""){
        
        
        if (_workContext.Admin == null)
            return RedirectToPage("managementLogin");
        using var _context = new LockerDbContext();

        var userQuery =  _context.Users.AsQueryable();
        if(type != 0)
            userQuery = userQuery.Where(x => x.Type == type);
        
        if(!string.IsNullOrEmpty(name))
            userQuery = userQuery.Where(x => x.Name.Contains(name));

        Users = await userQuery.OrderByDescending(x => x.CreatedOn).Skip((page - 1) * Utils.PageCount).Take(Utils.PageCount).ToListAsync();

        return Page();
    }

    public async Task<IActionResult> OnGetSetUserStatus(int id)
    {
        
        if (_workContext.Admin == null)
            return RedirectToPage("managementLogin");
        using var _context = new LockerDbContext();
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
        user.IsActive = !user.IsActive;
        user.ModifiedOn = DateTime.Now;
        await _context.SaveChangesAsync();
        return RedirectToPage("userList");
    }
}