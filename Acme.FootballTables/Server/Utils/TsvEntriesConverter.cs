using Acme.FootballTables.Server.Models;

namespace Acme.FootballTables.Server.Utils
{
    public static class TsvEntriesConverter
    {
        public static IEnumerable<LeagueTableEntry> GetTableEntriesFromTsv(IEnumerable<string> tsvLines, int tableId)
        {
            return tsvLines.Select(line =>
            {
                var elements = line.Split("\t");
                return new LeagueTableEntry
                {
                    Position = int.Parse(elements[0]),
                    Team = elements[1],
                    Points = int.Parse(elements[2]),
                    MatchesWon = int.Parse(elements[3]),
                    MatchesDrawn = int.Parse(elements[4]),
                    MatchesLost = int.Parse(elements[5]),
                    GoalsFor = int.Parse(elements[6]),
                    GoalsAgainst = int.Parse(elements[7]),
                    LeagueTableId = tableId
                };
            });
        }
    }
}
