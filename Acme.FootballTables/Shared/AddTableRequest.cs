using System.ComponentModel.DataAnnotations;

namespace Acme.FootballTables.Shared
{
    public class AddTableRequest
	{
        [Required, MinLength(3), MaxLength(1000)]
        public string Name { get; set; }

        [Required]
        public int SeasonId { get; set; }

        [MaxLength(10000)]
        public string? AdditionalInfo { get; set; }
    }
}
