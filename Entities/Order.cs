namespace Locker.Entities;

public class Order
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int DeliveryType { get; set; }
    public int OrderType { get; set; }
    public decimal TotalAmount { get; set; }
    
    public decimal Discount{get;set;}
    public int PayType { get; set; }
    
    public string? ReceiverOtpCode { get; set; }
    
    public string? SenderOtpCode { get; set; }
    
    public int SourceRegionId { get; set; }
    
    public int TargetRegionId { get; set; }
    
    public bool IsPaid { get; set; }
    
    public DateTime PaidDate { get; set; }
    public string? SenderName { get; set; }
    public string? SenderAddress { get; set; }
    public string? SenderAddressLat{get;set;}

    public string? SenderAddressLong{get;set;}
    public string? SenderPhone { get; set; }
    public string? SenderEmail { get; set; }
    public string? ReceiverName { get; set; }
    public string? ReceiverAddress { get; set; }
    public string? ReceiverPhone { get; set; }
    public string? ReceiverEmail { get; set; }
    
    public int CourierId { get; set; }
    
    public int? OldCourierId { get; set; }
    
    public bool IsTransferred { get; set; }

    public bool IsTransferCompleted { get; set; }

    public int Status{get;set;}

    public string? ReceiverAddressLat{get;set;}
    public string? ReceiverAddressLong{get;set;}
    public string? Note { get; set; }
    public DateTime? CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }

    public int ProblemId{get;set;}

    public string? ProblemDescription{get;set;}

    // İlişkilendirilmiş kullanıcı ve sipariş öğeleri
    public virtual User User { get; set; }
    public virtual Courier Courier { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; }
}