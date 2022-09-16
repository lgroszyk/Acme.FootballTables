using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acme.FootballTables.Shared
{
    public class GetTableResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public string Name { get; set; }
        public IEnumerable<TableEntry> Entries { get; set; }
    }

    public class TableEntry
    {
        public string Position { get; set; }

        public string Team { get; set; }

        public int Points { get; set; }

        public int MatchesPlayed { get; set; }

        public int MatchesWon { get; set; }

        public int MatchesDrawn { get; set; }

        public int MatchesLost { get; set; }

        public string GoalDifference { get; set; }
    }
}
