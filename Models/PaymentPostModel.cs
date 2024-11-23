namespace Locker.Models;

public class PaymentPostModel
{
    public string CardNumber { get; set; }
    public string ExpiryMonth { get; set; }
    public string ExpiryYear { get; set; }
    public string Cvv { get; set; }
    
    public bool PayWithCredit { get; set; }
}