using Locker.ExternalServices.BestWondService.Request;
using Locker.ExternalServices.BestWondService.Response;
using Locker.Models;

public interface IBestWondService{
   Task<CreateLockerOrderModel> SaveOrder(LockerCreateOrderRequest request);
}