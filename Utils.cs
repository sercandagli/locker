using System.Text;
using System.Security.Cryptography;
using Locker.Enums;

namespace Locker;


public static class Utils{

    public static int PageCount = 50;

    public static string GetDeliveryType(this int DeliveryType){
        switch((DeliveryType)DeliveryType){
            case Enums.DeliveryType.AddressToAddress:
            return "Adresten Adrese";
            case Enums.DeliveryType.AddressToLocker:
            return "Adresten Kargomata";
            case Enums.DeliveryType.LockerToLocker:
            return "Kargomattan Kargomata";
            case Enums.DeliveryType.LockerToAddress:
            return "Kargomattan Adrese";
            default:
            return "Tanımsız";

        }
    }

    public static string GetCourierOrderStatus(this int orderStatus, int deliveryType)
    {
        if (orderStatus == (int)OrderItemStatus.OrderCreated &&
            deliveryType is (int)DeliveryType.AddressToLocker or (int)DeliveryType.AddressToAddress)
            return "Teslim Al";

        if (orderStatus == (int)OrderItemStatus.AtSourceCabine && deliveryType is (int)DeliveryType.LockerToAddress or (int)DeliveryType.LockerToLocker)
            return "Teslim Al";

        if (orderStatus == (int)OrderItemStatus.AtCourier)
            return "Teslim Et";
        
        if (orderStatus == (int)OrderItemStatus.Delivered)
            return "Teslim Edildi";

        return string.Empty;
    }
    
    public static string GetUserOrderStatus(this int orderStatus, int deliveryType)
    {
        if (orderStatus == (int)OrderItemStatus.OrderCreated &&
            deliveryType is (int)DeliveryType.AddressToLocker or (int)DeliveryType.AddressToAddress)
            return "Siparişiniz Oluşturuldu";
        
        if (orderStatus == (int)OrderItemStatus.OrderCreated &&
            deliveryType is (int)DeliveryType.LockerToAddress or (int)DeliveryType.LockerToLocker)
            return "Paketi Otomata Teslim Ediniz";
        
        if (orderStatus == (int)OrderItemStatus.AtSourceCabine)
            return "Otomata Teslim Edildi";

        if (orderStatus == (int)OrderItemStatus.AtCourier)
            return "Dağıtımda";

        //burada hedef kargomattan paketi aldıktan sonra teslim edildi olacak
        if (orderStatus is (int)OrderItemStatus.AtTargetCabine or (int)OrderItemStatus.Delivered)
            return "Teslim Edildi";

        if (orderStatus == (int)OrderItemStatus.Cancelled)
            return "İptal Edildi";

        return string.Empty;
    }
    
    public static string GetAdminOrderStatus(this int orderStatus, int deliveryType)
    {
        if (orderStatus == (int)OrderItemStatus.OrderCreated &&
            deliveryType is (int)DeliveryType.AddressToLocker or (int)DeliveryType.AddressToAddress)
            return "Kurye Bekleniyor";
        
        if (orderStatus == (int)OrderItemStatus.OrderCreated &&
            deliveryType is (int)DeliveryType.LockerToAddress or (int)DeliveryType.LockerToLocker)
            return "Sipariş Oluşturuldu";
        
        if (orderStatus == (int)OrderItemStatus.AtSourceCabine)
            return "Kurye Bekleniyor";

        if (orderStatus == (int)OrderItemStatus.AtCourier)
            return "Dağıtımda";

        //burada hedef kargomattan paketi aldıktan sonra teslim edildi olacak
        if (orderStatus is (int)OrderItemStatus.AtTargetCabine or (int)OrderItemStatus.Delivered)
            return "Teslim Edildi";
        
        if (orderStatus == (int)OrderItemStatus.Cancelled)
            return "İptal Edildi";

        return string.Empty;
    }
    
    
    
    
    

    public static string GetOrderItemStatus(this int orderItemStatus, int deliveryType){
        if(orderItemStatus == (int) OrderItemStatus.OrderCreated){
            switch(deliveryType){
                case (int)DeliveryType.LockerToLocker:
                    return "Sipariş Oluşturuldu";
                case (int)DeliveryType.LockerToAddress:
                    return "Gönderime Hazır </br> (Otomata Bırakınız)";
                case (int)DeliveryType.AddressToLocker:
                    return "Gönderime Hazır </br> (Kurye Bekleniyor)";
                case (int)DeliveryType.AddressToAddress:
                    return "Gönderime Hazır </br> (Kurye Bekleniyor)";
            }
        }else if(orderItemStatus == (int)OrderItemStatus.Cancelled){
            return "İptal Edildi";
        }
        return string.Empty;
    }


    public static string GetUserType(this int UserType){
        switch((UserType)UserType){
            case Enums.UserType.Normal:
            return "Normal";
            case Enums.UserType.Commercial:
            return "Partner";
            case Enums.UserType.Partner:
            return "VIP Partner";
            default:
            return "Tanımsız";

        }
    }

     public static string HashPassword(this string password) {
 
            var md5 = new MD5CryptoServiceProvider();

            byte[] byteArray = Encoding.UTF8.GetBytes(password);
            byteArray = md5.ComputeHash(byteArray);
            var sb = new StringBuilder();
 
            foreach (byte ba in byteArray) {
                sb.Append(ba.ToString("x2").ToLower());
            }
 
            return sb.ToString(); 
      }
 
    
}