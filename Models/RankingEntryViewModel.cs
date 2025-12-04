namespace StepUp.Models
{
    public class RankingEntryViewModel
    {
        public int TeamId { get; set; }
        public string TeamName { get; set; } = string.Empty;
        public int Points { get; set; }
        public bool IsCurrentTeam { get; set; }
    }
}