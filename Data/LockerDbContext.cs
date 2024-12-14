namespace Locker.Data;

using Locker.Entities;
using Microsoft.EntityFrameworkCore;

public class LockerDbContext : DbContext
{
    public DbSet<Box> Boxes { get; set; }
    public DbSet<BoxAmount> BoxAmounts{get;set;}

    public DbSet<Cabine> Cabines { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Region> Regions { get; set; }
    public DbSet<User> Users { get; set; }
    
    public DbSet<SubRegion> SubRegions { get; set; }

    public DbSet<Courier> Couriers{get;set;}

    public DbSet<TransferPoint> TransferPoints{get;set;}
    
    public DbSet<Admin> Admins { get; set; }

    public DbSet<Coupon> Coupons{get;set;}


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Burada her bir tablo için ek konfigürasyonlar yapabilirsiniz

    
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Veritabanı bağlantı dizesi burada belirtilmelidir
        optionsBuilder.UseMySql("server=127.0.0.1;uid=cargodbuser;pwd=4505096Cargo!!;database=cargo", new MySqlServerVersion(new Version(8, 0, 21))); // MySQL versiyonuna göre ayarlayın
    }
}
