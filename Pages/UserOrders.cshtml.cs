using Locker;
using Locker.Data;
using Locker.Entities;
using Microsoft.EntityFrameworkCore;

namespace locker.Pages;

public class UserOrdersModel : BasePageModel
{
    public List<Order> Orders { get; set; }
    private readonly WorkContext _workContext;

    public UserOrdersModel(WorkContext workContext)
    {
        _workContext = workContext;
    }


    public async Task OnGet()
    {
        using var _context = new LockerDbContext();
        Orders = await _context.Orders.Where(x => x.UserId == _workContext.User.Id).Include(x => x.OrderItems)
            .ToListAsync();
    }
}