using Locker;
using Locker.Data;
using Locker.Entities;
using Locker.Enums;
using Locker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace locker.Pages;

public class CourierOrdersModel : BasePageModel
{
    private readonly WorkContext _workContext;
    
    public int TakeCount { get; set; }
    
    public int SaveCount { get; set; }
    public List<Region> Regions { get; set; }

    public CourierOrdersModel(WorkContext workContext)
    {
        _workContext = workContext;
    }
    public List<Cabine> Cabines{get;set;}
    public List<Order> Orders { get; set; }

    public async Task<IActionResult> OnGet(int deliveryAddressType,int regionId, int cabineId, int orderId,int isTransfer)
    {
        if (_workContext.Courier == null)
            return RedirectToPage("courierLogin");
        
        using var _context = new LockerDbContext();
        Cabines = await _context.Cabines.ToListAsync();
        Regions = await _context.Regions.ToListAsync();
        var ordersQuery =  _context.Orders.Include(x => x.OrderItems)
            .Where(x =>x.IsPaid && x.Status == (int)OrderStatus.Continue && x.ProblemId < 1).AsQueryable();
        if (orderId != 0)
        {
            Orders = await ordersQuery.Where(x => x.Id == orderId).ToListAsync();
            return Page();
        }

        if (deliveryAddressType == 1)
        {
            ordersQuery = ordersQuery.Where(x =>(
                x.DeliveryType == (int)DeliveryType.AddressToLocker ||
                x.DeliveryType == (int)DeliveryType.AddressToAddress) && x.SourceRegionId == regionId);
        }else if (deliveryAddressType == 2)
        {
            ordersQuery = ordersQuery.Where(x =>(
                x.DeliveryType == (int)DeliveryType.LockerToAddress ||
                x.DeliveryType == (int)DeliveryType.AddressToAddress) && x.TargetRegionId == regionId);
        }

        if (cabineId != 0)
        {
            ordersQuery = ordersQuery.Where(x =>
                x.OrderItems.FirstOrDefault().SourceLockerId == cabineId ||
                x.OrderItems.FirstOrDefault().TargetLockerId == cabineId);
        }

        if (isTransfer != 0)
        {
            ordersQuery = ordersQuery.Where(x =>
               x.IsTransferred && !x.IsTransferCompleted);
        }
        
        Orders = await ordersQuery.ToListAsync();
        var excepts = Orders.Where(x =>
            (x.DeliveryType == (int)DeliveryType.LockerToLocker || x.DeliveryType == (int)DeliveryType.LockerToAddress) &&
            x.OrderItems.FirstOrDefault(x => x.Status != (int)OrderItemStatus.Cancelled)?.Status == (int)OrderItemStatus.OrderCreated).ToList();


        var newOrderList = new List<Order>();
        Orders = Orders.Except(excepts).ToList();
        newOrderList.AddRange(Orders.Where(x => 
                (x.OrderItems.FirstOrDefault(x => x.Status != (int)OrderItemStatus.Cancelled)?.Status == (int)OrderItemStatus.AtSourceCabine && x.DeliveryType is (int)DeliveryType.LockerToLocker or (int)DeliveryType.LockerToAddress) || 
                                                     (x.OrderItems.FirstOrDefault(x => x.Status != (int)OrderItemStatus.Cancelled)?.Status == (int)OrderItemStatus.OrderCreated) && x.DeliveryType is (int)DeliveryType.AddressToAddress or (int)DeliveryType.AddressToLocker)
            .OrderByDescending(x => x.CreatedOn));
        TakeCount = newOrderList.Count;
        var nList = Orders.Where(x =>
            x.OrderItems.FirstOrDefault(x => x.Status != (int)OrderItemStatus.Cancelled)?.Status ==
            (int)OrderItemStatus.AtCourier).OrderBy(x => x.CreatedOn);

        newOrderList.AddRange(nList);
        SaveCount = newOrderList.Count - TakeCount;
        Orders = newOrderList;
        return Page();
    }

    public async Task<IActionResult> OnPostReport(CreateReportModel model){
        if (_workContext.Courier == null)
            return RedirectToPage("courierLogin");
        using var _context = new LockerDbContext();

        var order = await _context.Orders.FirstOrDefaultAsync(x => x.Id == model.OrderId);
        if(order == null)
        {
            return new JsonResult(new {
                Message = "Sipariş Bulunamadı"
            });
        }
        order.ProblemId = model.ProblemId;
        order.ProblemDescription = model.ProblemDescription;
        order.ModifiedOn = DateTime.Now;
        await _context.SaveChangesAsync();
        return new JsonResult(new {
            isSuccess = true
        });

    }

    public async Task<IActionResult> OnPostCompleteTransfers(TransferOrderModel model)
    {
        if (_workContext.Courier == null)
            return RedirectToPage("courierLogin");

        using var _context = new LockerDbContext();
        var ordersQuery = _context.Orders.Where(x => model.OrderIds.Contains(x.Id) && x.IsTransferred);
        await ordersQuery.ForEachAsync(x =>
        {
            x.IsTransferCompleted = true;
            x.ModifiedOn = DateTime.Now;
        });
        await _context.SaveChangesAsync();
        return new JsonResult(new
        {
            IsSuccess = true
        });
    }

}