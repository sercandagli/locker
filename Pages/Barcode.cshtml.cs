
using Locker.Data;
using Locker.Entities;
using Locker.Enums;
using Locker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WkHtmlToPdfDotNet;

namespace locker.Pages;

public class BarcodeModel : BasePageModel
{
    public List<BarcodeViewModel> BarcodeItems { get; set; }


    public async Task OnGet(List<int> oids)
    {
        BarcodeItems = new List<BarcodeViewModel>();
        using var _context = new LockerDbContext();

        foreach (var orderId in oids)
        {
            var order = await _context.Orders.Include(x => x.OrderItems).FirstOrDefaultAsync(x => x.Id == orderId);
            foreach (var orderItem in order.OrderItems.Where(x => x.Status != (int)OrderItemStatus.Cancelled && x.Status != (int)OrderItemStatus.Delivered))
            {


                if (oids.Count > 1)
                {
                    if (order.DeliveryType == (int)DeliveryType.LockerToLocker && orderItem.Status != (int)OrderItemStatus.AtSourceCabine)
                        continue;
                    if (order.DeliveryType == (int)DeliveryType.AddressToLocker && orderItem.Status == (int)OrderItemStatus.AtCourier)
                        continue;
                }

                var barcodeItem = new BarcodeViewModel
                {
                    SLSaveCode = orderItem.SLSaveCde ?? string.Empty,
                    SLPickCode = orderItem.SLPickCode ?? string.Empty,
                    TLSaveCode = orderItem.TLSaveCode ?? string.Empty,
                    TLPickCode = orderItem.TLPickCode ?? string.Empty,
                    SenderName = order.SenderName,
                    ReceiverName = order.ReceiverName,
                    SenderPhone = order.SenderPhone,
                    ReceiverPhone = order.ReceiverPhone,
                    SenderAddress = order.SenderAddress,
                    ReceiverAddress = order.ReceiverAddress,
                    BoxSize = orderItem.BoxSize,
                    OrderId = orderItem.Id
                };



                if (order.DeliveryType == (int)DeliveryType.LockerToLocker)
                {
                    barcodeItem.SLSaveCode = string.Empty;
                    barcodeItem.TLPickCode = string.Empty;
                }
                else if (order.DeliveryType == (int)DeliveryType.AddressToLocker)
                {
                    barcodeItem.SLSaveCode = string.Empty;
                    barcodeItem.SLPickCode = string.Empty;
                    barcodeItem.TLPickCode = string.Empty;
                }
                else if (order.DeliveryType == (int)DeliveryType.LockerToAddress)
                {
                    barcodeItem.SLSaveCode = string.Empty;
                    barcodeItem.TLSaveCode = string.Empty;
                    barcodeItem.TLPickCode = string.Empty;
                }


                var cabines = await _context.Cabines.ToListAsync();
                if (orderItem.SourceLockerId != null && orderItem.SourceLockerId != 0)
                    barcodeItem.SourceCabineName = cabines.FirstOrDefault(x => x.Id == orderItem.SourceLockerId).Name;
                if (orderItem.TargetLockerId != null && orderItem.TargetLockerId != 0)
                    barcodeItem.TargetCabineName = cabines.FirstOrDefault(x => x.Id == orderItem.TargetLockerId).Name;

                barcodeItem.IsTransfer = order.IsTransferred && !order.IsTransferCompleted;

                barcodeItem.TranferCourierName = _context.Couriers.FirstOrDefault(x => x.Id == order.CourierId)?.Name;

                if (!orderItem.IsPrinted)
                {
                    orderItem.IsPrinted = true;

                }

                barcodeItem.CreatedOn = orderItem.CreatedOn.ToString();
                BarcodeItems.Add(barcodeItem);

            }

            await _context.SaveChangesAsync();
        }

    }

    public async Task<IActionResult> OnPost(PrintModel model)
    {
        var converter = new BasicConverter(new PdfTools());
        model.Html = model.Html.Replace("\n", "").Replace("\"", "'");
        var doc = new HtmlToPdfDocument()
        {
            GlobalSettings = {
                Margins = new MarginSettings{
                         Bottom = 0,
                         Top = 0,
                         Left = 0,
                         Right = 0
                },
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Landscape,
                PaperSize = new PechkinPaperSize("130mm","70mm")
            },
            Objects = {
                new ObjectSettings() {
                    HtmlContent = model.Html,
                    WebSettings = { DefaultEncoding = "utf-8" }
                }
            }
        };

        var pdf = converter.Convert(doc);
        return File(pdf, "application/pdf", "siparisler.pdf");
    }
}