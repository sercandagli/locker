namespace Locker.Models;

public class OrderCsvModel
{
    public int OrderId { get; set; }
    
    
    public string Address { get; set; }
    
    public string Latitude { get; set; }
    
    public string Longitude { get; set; }
    
    public int GroupRegion { get; set; }
    
    public int StopColor{get;set;}
    
    public string CustomerNameOrCode { get; set; }
    
    public int TimeAtStop { get; set; }
    
    public string Phone { get; set; }
    
    public string Notes { get; set; }
}