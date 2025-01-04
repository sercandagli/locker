namespace Locker.Models
{
    public class AddCabineViewModel
    {
        public int? RegionId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string Lat { get; set; }
        public string Long { get; set; }
        public string Identifier { get; set; }

        public IFormFile File { get; set; }
    }
}