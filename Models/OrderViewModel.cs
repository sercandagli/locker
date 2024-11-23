namespace Locker.Models;



public class OrderViewModel
{

    public required List<CabineViewModel> Cabines{get;set;}

    public required List<BoxViewModel> Boxes{get;set;}
    
}

public class CabineViewModel{
    
    public int Id { get; set; }
    public required string Lat{get;set;}

    public required string Long{get;set;}

    public required string Name{get;set;}

    public string? Description{get;set;}
}



public class BoxViewModel{

    public required string Name{get;set;}

    public decimal Amount{get;set;}

    public decimal? PromotionAmount{get;set;}

    public bool IsPromotionActive{get;set;}

    public string? PromotionDescription{get;set;}

    public required string IconLink{get;set;}

    public int Id{get;set;}

}