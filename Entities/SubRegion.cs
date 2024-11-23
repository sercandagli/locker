namespace Locker.Entities;

public class SubRegion
{
    public int Id { get; set; }
    
    public int RegionId { get; set; }
    
    public string Name { get; set; }
    
    public string Coordinates { get; set; }
    
    public bool IsActive { get; set; }
    
    public DateTime CreatedOn { get; set; }

    public DateTime ModifiedOn { get; set; }
}