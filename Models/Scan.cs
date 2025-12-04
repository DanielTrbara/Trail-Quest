namespace StepUp.Models
{
    public class Scan
    {
        public int Id { get; set; }

        public int TeamId { get; set; }
        public Team Team { get; set; } = null!;

        public int PoiId { get; set; }   // QR-Code Standort (1,2,3,...)

        public DateTime ScannedAt { get; set; } = DateTime.UtcNow;
    }
}