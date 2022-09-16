using System.ComponentModel.DataAnnotations;

namespace Acme.FootballTables.Server.Models
{
	public class LeagueTableEntry
	{
        [Required, Key]
        public int Id { get; set; }

        [Required]
        public int LeagueTableId { get; set; }

        public LeagueTable LeagueTable;

        [Required]
        public int Position { get; set; }

        [Required, MaxLength(1000)]
        public string Team { get; set; }

        [Required]
        public int Points { get; set; }

        [Required]
        public int MatchesWon { get; set; }

        [Required]
        public int MatchesDrawn { get; set; }

        [Required]
        public int MatchesLost { get; set; }

        [Required]
        public int GoalsFor { get; set; }

        [Required]
        public int GoalsAgainst { get; set; }
    }
}
