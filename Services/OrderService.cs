using System.Text.Json;
using Locker.Data;
using Locker.Entities;
using Locker.Enums;
using Locker.ExternalServices.BestWondService.Request;
using Locker.Models;
using Microsoft.EntityFrameworkCore;

namespace Locker.Services;

public class OrderService : IOrderService
{
    private readonly IBestWondService _bestWondService;
    private readonly INotificationService _notificationService;
    private readonly WorkContext _workContext;
    private readonly IHttpContextAccessor _httpContext;

    public OrderService(IBestWondService bestWondService,
        INotificationService notificationService,
        WorkContext workContext,
        IHttpContextAccessor httpContext)
    {
        _bestWondService = bestWondService;
        _notificationService = notificationService;
        _workContext = workContext;
        _httpContext = httpContext;
    }

    public async Task CreateOrder(CreateOrderViewModel model)
    {
        using var _context = new LockerDbContext();
        User user = null;
        if (_workContext.User != null)
        {
            user = _workContext.User;
        }
        else
        {
            var newUser = new User()
            {
                Name = model.SenderName,
                Phone = model.SenderPhone,
                Email = model.SenderEmail,
                Type = (int)UserType.Normal,
                Credit = 0,
                AddressLat = model.SenderAddressLat,
                AddressLong = model.SenderAddressLong,
                Address = model.SenderAddress,
                CreatedOn = DateTime.Now,
                Password = "1a9ffHm93La.)opx78al_XHmLa".HashPassword()
            };
            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();
            user = newUser;
        }


        Cabine? sourceLocker = null;
        Cabine? targetLocker = null;
        int? regionId = 0;
        int? courierId = 0;


        if (model.DeliveryType is Enums.DeliveryType.LockerToLocker or Enums.DeliveryType.LockerToAddress)
        {
            sourceLocker =
                await _context.Cabines.FirstOrDefaultAsync(x => x.Id == Convert.ToInt16(model.SourceLockerName));
            regionId = sourceLocker?.RegionId;
        }


        if (model.DeliveryType is Enums.DeliveryType.LockerToLocker or Enums.DeliveryType.AddressToLocker)
        {
            targetLocker =
                await _context.Cabines.FirstOrDefaultAsync(x => x.Id == Convert.ToInt16(model.TargetLockerName));
            regionId = targetLocker?.RegionId;
        }

        if (model.DeliveryType is DeliveryType.AddressToAddress)
        {
            regionId = _context.Regions.FirstOrDefaultAsync().Result.Id;
        }

        if (regionId is not null)
            courierId = _context.Couriers.FirstOrDefaultAsync(x => x.RegionId == regionId).Result?.Id;


        var userType = user.Type;
        var orderItems = new List<OrderItem>();
        var now = DateTime.Now;
        var itemList = new List<OrderItemViewModel>();

        if (model.BoxId != 0)
        {
            itemList.Add(new OrderItemViewModel()
            {
                Id = model.BoxId,
                Count = 1
            });
        }
        else
        {
            itemList = JsonSerializer.Deserialize<List<OrderItemViewModel>>(model.Items);
        }

        foreach (var item in itemList)
        {
            var box = await _context.Boxes.FirstOrDefaultAsync(x => x.Id == item.Id);
            var boxAmount = await _context.BoxAmounts.FirstOrDefaultAsync(x =>
                x.DeliveryType == (int)model.DeliveryType && x.UserType == userType && x.BoxId == item.Id);
            decimal? amount = decimal.Zero;
            if (boxAmount is not null)
                amount = boxAmount.IsPromotionActive ? boxAmount.PromotionAmount : boxAmount.Amount;
            for (var i = 0; i < item.Count; i++)
            {
                orderItems.Add(new()
                {
                    BoxSize = box.Size,
                    Status = (int)OrderItemStatus.OrderCreated,
                    Amount = amount ?? decimal.Zero,
                    SourceLockerId = sourceLocker?.Id,
                    TargetLockerId = targetLocker?.Id,
                    CreatedOn = now,
                    ModifiedOn = now
                });
            }
        }

        var sourceRegionId = 0;
        var targetRegionId = 0;
        if (model.DeliveryType is DeliveryType.LockerToAddress or DeliveryType.LockerToLocker)
        {
            sourceRegionId = sourceLocker != null ? sourceLocker.RegionId.Value : 0;
        }
        else if (model.DeliveryType is DeliveryType.AddressToAddress or DeliveryType.AddressToLocker)
        {
            sourceRegionId = model.SelectedSourceRegionId;
        }


        if (model.DeliveryType is DeliveryType.AddressToLocker or DeliveryType.LockerToLocker)
        {
            targetRegionId = sourceLocker != null ? targetLocker.RegionId.Value : 0;
        }
        else if (model.DeliveryType is DeliveryType.AddressToAddress or DeliveryType.LockerToAddress)
        {
            targetRegionId = model.SelectedTargetRegionId;
        }


        var order = new Order
        {
            UserId = user.Id,
            DeliveryType = (int)model.DeliveryType,
            OrderType = (int)model.OrderType,
            TotalAmount = orderItems.Sum(x => x.Amount),
            SenderName = user.Name,
            SenderPhone = user.Phone,
            SenderEmail = user.Email,
            SenderAddress = user.Address,
            SenderAddressLat = user.AddressLat,
            SenderAddressLong = user.AddressLong,
            Note = model.Note,
            SourceRegionId = sourceRegionId,
            TargetRegionId = targetRegionId,
            CourierId = courierId ?? 0,
            Status = (int)OrderStatus.Continue,
            ReceiverName = model.ReceiverName,
            ReceiverPhone = model.ReceiverPhone,
            ReceiverEmail = model.ReceiverEmail,
            ReceiverAddress = model.ReceiverAddress,
            ReceiverAddressLat = model.ReceiverAddressLat,
            ReceiverAddressLong = model.ReceiverAddressLong,
            CreatedOn = now,
            ModifiedOn = now,
            OrderItems = orderItems
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        _httpContext.HttpContext.Session.SetInt32("orderId", order.Id);
        foreach (var item in order.OrderItems)
        {
            var orderId = order.Id.ToString() + "_" + item.Id.ToString();
            if (sourceLocker is not null)
            {
                var sourceLockerApiResponse = await _bestWondService.SaveOrder(new LockerCreateOrderRequest
                {
                    OrderId = orderId,
                    BoxSize = item.BoxSize,
                    DeviceNumber = sourceLocker.Identifier
                });

                if (sourceLockerApiResponse is not null && sourceLockerApiResponse.IsSuccess)
                {
                    item.SLSaveCde = sourceLockerApiResponse.SaveCode;
                    item.SLPickCode = sourceLockerApiResponse.PickCode;
                }
            }

            if (targetLocker is not null)
            {
                var targetLockerApiResponse = await _bestWondService.SaveOrder(new LockerCreateOrderRequest
                {
                    OrderId = orderId + "_1",
                    BoxSize = item.BoxSize,
                    DeviceNumber = targetLocker.Identifier
                });

                if (targetLockerApiResponse is not null && targetLockerApiResponse.IsSuccess)
                {
                    item.TLSaveCode = targetLockerApiResponse.SaveCode;
                    item.TLPickCode = targetLockerApiResponse.PickCode;
                }
            }

            if (model.DeliveryType is Enums.DeliveryType.LockerToAddress or Enums.DeliveryType.AddressToAddress)
            {
                var otpResult = await _notificationService.SendOtp(model.ReceiverPhone);
                if (!string.IsNullOrEmpty(otpResult))
                    item.OtpCode = otpResult;
            }
        }

        await _context.SaveChangesAsync();

        await _notificationService.SendOrderCreatedNotification(model.SenderPhone);
    }

    public async Task<OrderViewModel> PrepareOrderPage(int deliveryType, int userType)
    {
        using var _context = new LockerDbContext();

        var cabines = await _context.Cabines.ToListAsync();

        var model = new OrderViewModel
        {
            Boxes = [],
            Cabines = cabines.Select(x => new CabineViewModel
            {
                Name = x.Name,
                Description = x.Description,
                Lat = x.Lat,
                Id = x.Id,
                Long = x.Long
            }).ToList()
        };

        var boxes = await _context.Boxes.ToListAsync();
        var boxAmounts = await _context.BoxAmounts.Where(x => x.DeliveryType == deliveryType && x.UserType == userType)
            .ToListAsync();


        foreach (var box in boxes)
        {
            var boxAmount = boxAmounts.FirstOrDefault(x => x.BoxId == box.Id);
            if (boxAmount != null)
            {
                model.Boxes.Add(new BoxViewModel
                {
                    Name = box.Size,
                    IconLink = box.IconLink,
                    Amount = boxAmount.Amount,
                    PromotionAmount = boxAmount.PromotionAmount,
                    PromotionDescription = boxAmount.PromotionDescription,
                    IsPromotionActive = boxAmount.IsPromotionActive,
                    Id = box.Id
                });
            }
        }

        return model;
    }
}