using Locker.Data;
using Locker.Entities;
using locker.Pages;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Locker.Pages;


public class SetTransferPointModel : BasePageModel{
    

    public string? PrevLat{get;set;}

    public string? PrevLong{get;set;}
    public SetTransferPointModel(){

    }

    public async Task OnGet(){
        using var _context = new LockerDbContext();
        var transferPoint = await _context.TransferPoints.FirstOrDefaultAsync();

        if(transferPoint != null)
        {
            PrevLat = transferPoint.Lat;
            PrevLong = transferPoint.Long;
        }
    }


    public async Task OnPost(TransferPoint transferPoint){
        using var _context = new LockerDbContext();
        var prev = await _context.TransferPoints.FirstOrDefaultAsync();

        if(prev != null)
        {
            prev.Lat = transferPoint.Lat;
            prev.Long = transferPoint.Long;
        }else{
            await _context.TransferPoints.AddAsync(new TransferPoint{
                Lat = transferPoint.Lat,
                Long = transferPoint.Long
            });
        }

        PrevLat = transferPoint.Lat;
        PrevLong = transferPoint.Long;

        await _context.SaveChangesAsync();
    }
}