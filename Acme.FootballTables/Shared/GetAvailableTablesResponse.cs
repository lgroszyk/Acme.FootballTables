namespace Acme.FootballTables.Shared
{
    public class GetAvailableTablesResponse
    {
        public IEnumerable<SeasonTablesSet> SeasonTablesSets { get; set; }
    }

    public class SeasonTablesSet
    {
        public int SeasonId { get; set; }
        public string SeasonName { get; set; }
        public IEnumerable<SeasonTableInfo> Tables { get; set; }
    }

    public class SeasonTableInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
