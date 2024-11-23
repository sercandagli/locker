namespace Locker.Entities;

public class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public string BoxSize { get; set; }
    public decimal Amount { get; set; }
    
    public bool IsPrinted { get; set; }
    public int? SourceLockerId { get; set; }
    public string? SLPickCode { get; set; }
    public string? SLSaveCde { get; set; }
    public string? TLPickCode { get; set; }
    public string? TLSaveCode { get; set; }
    public int? TargetLockerId{get;set;}
    public int Status { get; set; }
    public string? OtpCode{get;set;}
    public DateTime? CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }

    // İlişkilendirilmiş sipariş
    public virtual Order Order { get; set; }
}