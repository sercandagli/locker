namespace Locker.Entities;


public class Coupon{
    public int Id{get;set;}

    public int Amount{get;set;}

    public string Code{get;set;}

    public bool IsUsed{get;set;}
}