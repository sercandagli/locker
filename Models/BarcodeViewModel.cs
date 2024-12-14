namespace Locker.Models;

public class BarcodeViewModel{
    public int OrderId{get;set;}
    public string SLSaveCode { get; set; }
    public string SLPickCode { get; set; }

    public string SenderName{get;set;}

    public string ReceiverName{get;set;}

    public string SenderPhone{get;set;}

    public string ReceiverPhone{get;set;}
    
    public string CreatedOn{get;set;}
    
    public string TLSaveCode { get; set; }
    public string TLPickCode { get; set; }
    
    public string SourceCabineName { get; set; }
    
    public string TargetCabineName { get; set; }
    
    public bool IsTransfer { get; set; }

    public string BoxSize{get;set;}

    public string SenderAddress{get;set;}

    public string ReceiverAddress{get;set;}
    
    public string TranferCourierName { get; set; }
}
