using Acme.FootballTables.Server.Models;
using Acme.FootballTables.Server.Utils;

namespace Acme.FootballTables.Server.Data
{
    public class SampleDatabaseSeeder
    {
        public static void ClearAndSeedFootballTables(ApplicationDbContext databaseContext)
        {
            databaseContext.LeagueTableEntries.RemoveRange(databaseContext.LeagueTableEntries);
            databaseContext.LeagueTables.RemoveRange(databaseContext.LeagueTables);
            databaseContext.Seasons.RemoveRange(databaseContext.Seasons);
            databaseContext.SaveChanges();

            var seasons = new[] 
            { 
                new Season { Name = "2020/2021", StartYear = 2020, EndYear = 2021 },
                new Season { Name = "2019/2020", StartYear = 2019, EndYear = 2020 },
                new Season { Name = "2018/2019", StartYear = 2018, EndYear = 2019 }
            };
            databaseContext.Seasons.AddRange(seasons);
            databaseContext.SaveChanges();

            var tables = new[] 
            { 
                new LeagueTable { Name = "Ekstraklasa", SeasonId = seasons[0].Id, AdditionalInfo = "" },
                new LeagueTable { Name = "I liga", SeasonId = seasons[0].Id, AdditionalInfo = "" },
                new LeagueTable { Name = "II liga", SeasonId = seasons[0].Id, AdditionalInfo = "" },

                new LeagueTable { Name = "Ekstraklasa", SeasonId = seasons[1].Id, AdditionalInfo = "" },
                new LeagueTable { Name = "I liga", SeasonId = seasons[1].Id, AdditionalInfo = "" },
                new LeagueTable { Name = "II liga", SeasonId = seasons[1].Id, AdditionalInfo = "" },

                new LeagueTable { Name = "Ekstraklasa", SeasonId = seasons[2].Id, AdditionalInfo = "" },
                new LeagueTable { Name = "I liga", SeasonId = seasons[2].Id, AdditionalInfo = "" },
                new LeagueTable { Name = "II liga", SeasonId = seasons[2].Id, AdditionalInfo = "" },
            };
            databaseContext.LeagueTables.AddRange(tables);
            databaseContext.SaveChanges();

            var entriesItems = new[]
            {
                new { Table = 0, Source = @"Data\Samples\Ekstraklasa_2020_2021.txt" },
                new { Table = 1, Source = @"Data\Samples\ILiga_2020_2021.txt" },
                new { Table = 2, Source = @"Data\Samples\IILiga_2020_2021.txt" },
                new { Table = 3, Source = @"Data\Samples\Ekstraklasa_2019_2020.txt" },
                new { Table = 4, Source = @"Data\Samples\ILiga_2019_2020.txt" },
                new { Table = 5, Source = @"Data\Samples\IILiga_2019_2020.txt" },
                new { Table = 6, Source = @"Data\Samples\Ekstraklasa_2018_2019.txt" },
                new { Table = 7, Source = @"Data\Samples\ILiga_2018_2019.txt" },
                new { Table = 8, Source = @"Data\Samples\IILiga_2018_2019.txt" },
            };
            foreach (var entriesItem in entriesItems)
            {
                var entriesData = File.ReadAllLines(entriesItem.Source);
                var entries = TsvEntriesConverter.GetTableEntriesFromTsv(entriesData, tables[entriesItem.Table].Id);
                databaseContext.LeagueTableEntries.AddRange(entries);
            }
            databaseContext.SaveChanges();
        }
    }
}
