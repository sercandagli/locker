namespace Locker.Entities;

public class User
{
    public int Id { get; set; }
    public int Type { get; set; }
    public string Name { get; set; }
    public string? Address { get; set; }
    
    public string? AddressLat { get; set; }
    
    public string? AddressLong { get; set; }
    public decimal Credit { get; set; }

    public bool ForgotPasswordProcess{get;set;}
    
    public bool IsActive { get; set; }
    
    public decimal PromotionCredit { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string? Password { get; set; }
    public DateTime? CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }

    // İlişkilendirilmiş siparişler
    public virtual ICollection<Order>? Orders { get; set; }
}
