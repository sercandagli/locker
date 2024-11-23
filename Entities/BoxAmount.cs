using System;
using System.Collections.Generic;
namespace Locker.Entities;


public class BoxAmount
{
    public int Id { get; set; }
    public required int BoxId { get; set; }
    public required int DeliveryType { get; set; }
    public required int UserType { get; set; }
    public required decimal Amount { get; set; }
    public decimal? PromotionAmount { get; set; }

    public string? PromotionDescription{get;set;}

    public required bool IsPromotionActive{get;set;}

    public DateTime? ModifiedOn{get;set;}

    // İlişkilendirilmiş kutu
    public virtual Box Box { get; set; }
}








