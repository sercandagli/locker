namespace Locker.Entities;
public class Box
{
    public int Id { get; set; }
    public string Size { get; set; }
    public string IconLink { get; set; }

    // İlişkilendirilmiş kampanyalar
    public virtual ICollection<BoxAmount> BoxAmounts { get; set; }
}
