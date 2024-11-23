
using Locker.Data;
using Locker.Enums;
using Microsoft.EntityFrameworkCore;

namespace locker.Pages;

public class BarcodeModel : BasePageModel
{
    public string SLSaveCode { get; set; }
    public string SLPickCode { get; set; }
    
    public string TLSaveCode { get; set; }
    public string TLPickCode { get; set; }
    
    public string SourceCabineName { get; set; }
    
    public string TargetCabineName { get; set; }
    
    public bool IsTransfer { get; set; }
    
    public string TranferCourierName { get; set; }
    
    
    public async Task OnGet(int orderItemId)
    {
        using var _context = new LockerDbContext();

        var orderItem = await _context.OrderItems.FirstOrDefaultAsync(x => x.Id == orderItemId);
        var order = await _context.Orders.FirstOrDefaultAsync(x => x.Id == orderItem.OrderId);
        SLSaveCode = orderItem.SLSaveCde ?? string.Empty;
        SLPickCode = orderItem.SLPickCode ?? string.Empty;
        TLSaveCode = orderItem.TLSaveCode ?? string.Empty;
        TLPickCode = orderItem.TLPickCode ?? string.Empty;


        if (order.DeliveryType == (int)DeliveryType.LockerToLocker)
        {
            SLSaveCode = string.Empty;
            TLPickCode = string.Empty;
        }else if (order.DeliveryType == (int)DeliveryType.AddressToLocker)
        {
            SLSaveCode = string.Empty;
            SLPickCode = string.Empty;
            TLPickCode = string.Empty;
        }else if (order.DeliveryType == (int)DeliveryType.LockerToAddress)
        {
            SLSaveCode = string.Empty;
            TLSaveCode = string.Empty;
            TLPickCode = string.Empty;
        }
        
        
        var cabines = await _context.Cabines.ToListAsync();
        if (orderItem.SourceLockerId != null && orderItem.SourceLockerId != 0)
            SourceCabineName = cabines.FirstOrDefault(x => x.Id == orderItem.SourceLockerId).Name;
        if(orderItem.TargetLockerId != null && orderItem.TargetLockerId != 0)
            TargetCabineName = cabines.FirstOrDefault(x => x.Id == orderItem.TargetLockerId).Name;

        IsTransfer = order.IsTransferred && !order.IsTransferCompleted;

        TranferCourierName = _context.Couriers.FirstOrDefault(x => x.Id == order.CourierId).Name;

        if (!orderItem.IsPrinted)
        {
            orderItem.IsPrinted = true;
            await _context.SaveChangesAsync();

        }

    }
}