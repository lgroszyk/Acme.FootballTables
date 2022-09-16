using System.ComponentModel.DataAnnotations;

namespace Acme.FootballTables.Server.Models
{
	public class LeagueTable
	{
        [Required, Key]
        public int Id { get; set; }

        [Required, MaxLength(1000)]
        public string Name { get; set; }

        [Required]
        public int SeasonId { get; set; }

        public Season Season { get; set; }

        public ICollection<LeagueTableEntry> TableEntries { get; set; }

        [MaxLength(10000)]
        public string AdditionalInfo { get; set; }
    }
}
