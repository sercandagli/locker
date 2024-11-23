using Locker.Models;

namespace Locker.Services;


public interface IOrderService
{
    Task<OrderViewModel> PrepareOrderPage(int deliveryType,int userType);

    Task CreateOrder(CreateOrderViewModel model);
}