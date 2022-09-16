using System.ComponentModel.DataAnnotations;

namespace Acme.FootballTables.Shared
{
    public class EditSeasonRequest
	{
        [Required, MaxLength(1000), MinLength(3)]
        public string Name { get; set; }

        public int StartYear { get; set; }

        public int EndYear { get; set; }
    }
}
