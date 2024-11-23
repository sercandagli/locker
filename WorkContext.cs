using Locker.Entities;

namespace Locker;

public class WorkContext  
{
    public User? User { get; set; }
    
    public Courier? Courier { get; set; }
    
    public Admin? Admin { get; set; }
}