namespace Locker.Models;

public class SendOtpModel
{
    public int OrderId { get; set; }
    
    public bool SendSender { get; set; }
    public bool SendReceiver{ get; set; }

    
    public string SenderPhone { get; set; }
    
    public string ReceiverPhone { get; set; }

}