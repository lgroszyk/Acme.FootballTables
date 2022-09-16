using System.ComponentModel.DataAnnotations;

namespace Acme.FootballTables.Server.Models
{
	public class Season
	{
		[Required, Key]
		public int Id { get; set; }

		[Required, MaxLength(1000)]
		public string Name { get; set; }

		public int StartYear { get; set; }

		public int EndYear { get; set; }

        public ICollection<LeagueTable> LeaguesTables { get; set; }
    }
}
