namespace Locker.Entities;

public class Courier
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public int RegionId { get; set; }
    
    public string Password { get; set; }
    
    public bool IsAvailable { get; set; }
    
    public virtual Region? Region { get; set; }
}