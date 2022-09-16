namespace Acme.FootballTables.Shared
{
    public class GetSeasonResponse
	{
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Name { get; set; }
        public int StartYear { get; set; }
        public int EndYear { get; set; }
    }
}
