using System.ComponentModel.DataAnnotations;

namespace Acme.FootballTables.Shared
{
	public class EditTableRequest
	{
		[Required]
		public string Entries { get; set; }
	}
}
