using System.ComponentModel.DataAnnotations;

namespace StepUp.Models
{
    public class Team
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Scan> Scans { get; set; } = new List<Scan>();
    }
}