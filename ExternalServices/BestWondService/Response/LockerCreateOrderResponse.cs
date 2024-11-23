

using Locker.Models;

namespace Locker.ExternalServices.BestWondService.Response;


public class LockerCreateOrderResponse : CreateLockerOrderModel {
    public int code{get;set;}

    public string msg{get;set;}

    public SavedOrderData data{get;set;}


}

public class SavedOrderData{
    public string box_name{get;set;}

    public string save_code{get;set;}

    public string pick_code{get;set;}

    public string status{get;set;}

    public string msg{get;set;}

}