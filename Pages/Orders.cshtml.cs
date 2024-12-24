using System.Text;
using CsvHelper;
using Locker.Data;
using Locker.Entities;
using Locker.Enums;
using Locker.Models;
using locker.Pages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Locker.Pages;



public class OrdersModel : BasePageModel
{

    private readonly WorkContext _workContext;

    public OrdersModel(WorkContext workContext)
    {
        _workContext = workContext;
    }

    public List<Order> Orders { get; set; }
    public List<Courier> Couriers { get; set; }

    public async Task<IActionResult> OnGet(bool? hasReport, bool? isTransfer, int? orderType, int? deliveryType, int? courierId, int? orderId)
    {

        if (_workContext.Admin == null)
            return RedirectToPage("managementLogin");
        using var _context = new LockerDbContext();

        var ordersQuery = _context.Orders.Include(x => x.OrderItems)
            .Where(x => x.Status == (int)OrderStatus.Continue && x.IsPaid).AsQueryable();


        if (orderId.HasValue && orderId > 0)
        {
            Orders = await _context.Orders.Include(x => x.OrderItems)
                  .Where(x => x.Id == orderId.Value).ToListAsync();
            Couriers = await _context.Couriers.ToListAsync();

            return Page();
        }

        if (hasReport.HasValue)
        {
            ordersQuery = ordersQuery.Where(x => x.ProblemId > 0);
        }

        if (isTransfer.HasValue)
        {
            ordersQuery = ordersQuery.Where(x => x.IsTransferred == true);
        }

        if (deliveryType.HasValue && deliveryType != 0)
        {
            ordersQuery = ordersQuery.Where(x => x.DeliveryType == deliveryType.Value);
        }

        if (courierId.HasValue && courierId != 0)
        {
            ordersQuery = ordersQuery.Where(x => x.CourierId == courierId.Value);
        }

        if (orderType.HasValue)
        {
            ordersQuery = ordersQuery.Where(x => x.OrderType == orderType.Value);

        }
        Orders = await ordersQuery.OrderByDescending(x => x.ModifiedOn)
           .ToListAsync();
        Couriers = await _context.Couriers.ToListAsync();

        return Page();
    }

    public async Task<IActionResult> OnPostResolveReport(CreateReportModel model)
    {
        if (_workContext.Admin == null)
            return RedirectToPage("managementLogin");
        using var _context = new LockerDbContext();

        var order = await _context.Orders.FirstOrDefaultAsync(x => x.Id == model.OrderId);
        if (order == null)
        {
            return new JsonResult(new
            {
                message = "Sipariş Bulunamadı"
            });
        }

        order.ProblemId = 0;
        order.ProblemDescription = string.Empty;
        order.ModifiedOn = DateTime.Now;
        await _context.SaveChangesAsync();
        return new JsonResult(new
        {
            isSuccess = true
        });

    }

    public async Task<IActionResult> OnPostTransfer(TransferOrderModel model)
    {

        if (_workContext.Admin == null)
            return RedirectToPage("managementLogin");
        await using var _context = new LockerDbContext();
        var orders = await _context.Orders.Where(x => model.OrderIds.Contains(x.Id) && !x.IsTransferred && x.Status == (int)OrderStatus.Continue).ToListAsync();

        foreach (var order in orders)
        {
            order.IsTransferred = true;
            order.OldCourierId = order.CourierId;
            order.CourierId = model.CourierId;
            order.ModifiedOn = DateTime.Now;
        }

        await _context.SaveChangesAsync();
        Orders = await _context.Orders.Include(x => x.OrderItems)
            .Include(x => x.Courier)
            .Where(x => x.Status == (int)OrderStatus.Continue)
            .OrderByDescending(x => x.ModifiedOn)
            .ToListAsync();

        return RedirectToPage("orders");
    }

    public async Task<IActionResult> OnGetExportCsv()
    {

        if (_workContext.Admin == null)
            return RedirectToPage("home");
        using var _context = new LockerDbContext();
        var lockers = await _context.Cabines.ToListAsync();
        var orderModel = await _context.Orders.Include(x => x.OrderItems).Where(x => x.Status == (int)OrderStatus.Continue).ToListAsync();
        var orderCsvModel = new List<OrderCsvModel>();




        // otomata bırakılanlar -> otomattan adrese, otomattan otomata

        var sourceLockerBasedGroupQuery = orderModel.Where(x =>
                x.DeliveryType is (int)DeliveryType.LockerToAddress or (int)DeliveryType.LockerToLocker
                && x.OrderItems.Any(x => x.Status == (int)OrderItemStatus.AtSourceCabine) && (!x.IsTransferred || x.IsTransferCompleted)).SelectMany(x => x.OrderItems)
            .ToList();


        var sourceLockerBasedGroups = sourceLockerBasedGroupQuery.GroupBy(x => x.SourceLockerId);

        foreach (var sourceLockerBasedGroup in sourceLockerBasedGroups)
        {
            var locker = lockers.FirstOrDefault(x => x.Id == sourceLockerBasedGroup.Key);
            orderCsvModel.Add(new OrderCsvModel()
            {
                Latitude = locker.Lat,
                Longitude = locker.Long,
                TimeAtStop = 3,
                GroupRegion = sourceLockerBasedGroup.Count(),
                StopColor = 8
            });
        }

        //otomata bırakılacaklar -> adresten otomata, otomattan otomata

        var targetLockerBasedGroupQuery = orderModel.Where(x =>
                x.DeliveryType is (int)DeliveryType.AddressToLocker or (int)DeliveryType.LockerToLocker
                && x.OrderItems.Any(x => x.Status == (int)OrderItemStatus.AtCourier) && (!x.IsTransferred || x.IsTransferCompleted)).SelectMany(x => x.OrderItems)
            .ToList();

        var targetLockerBasedGroups = targetLockerBasedGroupQuery.GroupBy(x => x.TargetLockerId);

        foreach (var targetLockerBasedGroup in targetLockerBasedGroups)
        {
            var locker = lockers.FirstOrDefault(x => x.Id == targetLockerBasedGroup.Key);
            orderCsvModel.Add(new OrderCsvModel()
            {
                Latitude = locker.Lat,
                Longitude = locker.Long,
                TimeAtStop = 3,
                GroupRegion = targetLockerBasedGroup.Count(),
                StopColor = 8
            });
        }






        var fromAddressBasedGroupsQuery = orderModel.Where(x =>
                x.DeliveryType is (int)DeliveryType.AddressToLocker or (int)DeliveryType.AddressToAddress
                && x.OrderItems.Any(x => x.Status == (int)OrderItemStatus.OrderCreated) && (!x.IsTransferred || x.IsTransferCompleted)).SelectMany(x => x.OrderItems)
            .ToList();


        foreach (var fromAddressBasedGroup in fromAddressBasedGroupsQuery.GroupBy(x => x.OrderId))
        {
            var order = orderModel.FirstOrDefault(x => x.Id == fromAddressBasedGroup.FirstOrDefault().OrderId);

            orderCsvModel.Add(new OrderCsvModel()
            {
                OrderId = order.Id,
                Address = order.SenderAddress,
                Latitude = order.SenderAddressLat,
                Longitude = order.SenderAddressLong,
                TimeAtStop = order.OrderType == (int)UserType.Partner ? 5 : 3,
                GroupRegion = fromAddressBasedGroup.Count(),
                StopColor = 12,
                CustomerNameOrCode = order.SenderName,

                Notes = order.Note,
                Phone = order.SenderPhone,
            });
        }



        var toAddressBasedGroupsQuery = orderModel.Where(x =>
                x.DeliveryType is (int)DeliveryType.LockerToAddress or (int)DeliveryType.AddressToAddress
                && x.OrderItems.Any(x => x.Status == (int)OrderItemStatus.AtCourier) && (!x.IsTransferred || x.IsTransferCompleted)).SelectMany(x => x.OrderItems)
            .ToList();


        foreach (var toAddressBasedGroup in toAddressBasedGroupsQuery.GroupBy(x => x.OrderId))
        {
            var order = orderModel.FirstOrDefault(x => x.Id == toAddressBasedGroup.FirstOrDefault().OrderId);

            orderCsvModel.Add(new OrderCsvModel()
            {
                Address = order.ReceiverAddress,
                Latitude = order.ReceiverAddressLat,
                Longitude = order.ReceiverAddressLat,
                TimeAtStop = 3,
                Notes = order.Note,
                CustomerNameOrCode = order.ReceiverName,
                Phone = order.ReceiverPhone,
                GroupRegion = toAddressBasedGroup.Count(),
                StopColor = 6,
                OrderId = order.Id
            });
        }

        var transferOrderQuery = orderModel.Where(x => x.IsTransferred && !x.IsTransferCompleted).SelectMany(x => x.OrderItems)
           .ToList();

        if (transferOrderQuery.Any())
        {
            var transferPoint = await _context.TransferPoints.FirstOrDefaultAsync();
            orderCsvModel.Add(new OrderCsvModel()
            {
                Latitude = transferPoint.Lat,
                Longitude = transferPoint.Long,
                TimeAtStop = 3,
                GroupRegion = transferOrderQuery.Count(),
                StopColor = 20

            });
        }




        var path = Environment.CurrentDirectory + "/orders.csv";
        using (var sw = new StreamWriter(path, false, new UTF8Encoding(true)))
        using (var cw = new CsvWriter(sw))
        {
            cw.WriteHeader<OrderCsvModel>();
            cw.NextRecord();
            foreach (var order in orderCsvModel)
            {
                cw.WriteRecord(order);
                cw.NextRecord();
            }
        }

        return File(System.IO.File.OpenRead(path), "application/octet-stream",
            $"orders-{DateTime.Now.Ticks}.csv");
    }

    public async Task<IActionResult> OnGetExportExcel(List<int> orderIds)
    {

        if (_workContext.Admin == null)
            return RedirectToPage("home");
        using var _context = new LockerDbContext();
        var lockers = await _context.Cabines.ToListAsync();
        var orderModel = await _context.Orders.Include(x => x.OrderItems).Where(x => orderIds.Contains(x.Id)).ToListAsync();



        var path = Environment.CurrentDirectory + "/orders.csv";
        using (var sw = new StreamWriter(path, false, new UTF8Encoding(true)))
        using (var cw = new CsvWriter(sw))
        {
            cw.WriteHeader<Order>();
            cw.NextRecord();
            foreach (var order in orderModel)
            {
                cw.WriteRecord(order);
                cw.NextRecord();
            }
        }

        return File(System.IO.File.OpenRead(path), "application/octet-stream",
            $"orders-{DateTime.Now.Ticks}.csv");
    }




}
