namespace Locker.Entities;

public class Cabine
{
    public int Id { get; set; }
    public int? RegionId { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string Lat { get; set; }
    public string Long { get; set; }
    public string Identifier { get; set; }

    public string? ImagePath { get; set; }

    public bool IsActive { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public virtual Region? Region { get; set; }
}