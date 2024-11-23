namespace Locker.Models;

public class CreateLockerOrderModel {
     public bool IsSuccess{get;set;}

    public string? BoxName{get;set;}

    public string? SaveCode{get;set;}

    public string? PickCode{get;set;}

    public string? ErrorMessage{get;set;}
}