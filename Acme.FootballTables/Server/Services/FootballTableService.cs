using Acme.FootballTables.Server.Cache;
using Acme.FootballTables.Server.Data;
using Acme.FootballTables.Server.Models;
using Acme.FootballTables.Server.Utils;
using Acme.FootballTables.Shared;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Net;

namespace Acme.FootballTables.Server.Services
{
    public class FootballTableService : IFootballTableService
    {
        private readonly ApplicationDbContext databaseContext;
        private readonly CacheContext cacheContext;
        private readonly IMapper mapper;

        public FootballTableService(
            ApplicationDbContext databaseContext,
            CacheContext cacheContext,
            IMapper mapper)
        {
            this.databaseContext = databaseContext;
            this.cacheContext = cacheContext;
            this.mapper = mapper;
        }

        public async Task<ServiceActionResult<GetAvailableTablesResponse>> GetAvailableTablesAsync()
        {
            //var tablesQuery = () => mapper.ProjectTo<SeasonTablesSet>(databaseContext.Seasons
            //    .OrderByDescending(season => season.EndYear));

            var seasons = await cacheContext.GetOrAddAsync("SeasonsOrderedDescendingByEndYearWithTablesIncluded",
                GetSeasonsOrderedDescendingByEndYearWithTablesIncludedQuery());

            var responseBody = new GetAvailableTablesResponse
            {
                SeasonTablesSets = mapper.Map<IEnumerable<SeasonTablesSet>>(seasons)
            };

            return new ServiceActionResult<GetAvailableTablesResponse>(
                HttpStatusCode.OK, responseBody);
        }

        public async Task<ServiceActionResult<GetAvailableSeasonsResponse>> GetAvailableSeasonsAsync()
        {
            var seasons = await cacheContext.GetOrAddAsync("SeasonsOrderedDescendingByEndYearWithTablesIncluded",
                GetSeasonsOrderedDescendingByEndYearWithTablesIncludedQuery());

            var responseBody = new GetAvailableSeasonsResponse
            {
                Seasons = mapper.Map<IEnumerable<OptionEntry>>(seasons)
            };

            return new ServiceActionResult<GetAvailableSeasonsResponse>(
                HttpStatusCode.OK, responseBody);
        }

        public async Task<ServiceActionResult<GetTableResponse>> GetTableAsync(int id)
        {
            var table = await cacheContext.GetOrAddAsync($"LeagueTableWithItsEntriesAndRelatedSeasonIncluded_{id}", 
                GetLeagueTableWithItsEntriesAndSeasonIncluded(id));

            if (table == null)
            {
                var notFoundResponseBody = new GetTableResponse
                {
                    Success = false,
                    Message = "Sorry, there is not given table data available."
                };
                return new ServiceActionResult<GetTableResponse>(
                    HttpStatusCode.NotFound, notFoundResponseBody);
            }

            var entries = table.TableEntries
                .Select(entry => mapper.Map<TableEntry>(entry));
            var successResponseBody = new GetTableResponse
            {
                Success = true,
                Name = $"{table.Name} - {table.Season.Name}",
                Entries = entries
            };
            return new ServiceActionResult<GetTableResponse>(
                HttpStatusCode.OK, successResponseBody);
        }

        public async Task<ServiceActionResult<AddTableResponse>> AddTableAsync(AddTableRequest request)
        {
            var seasons = await cacheContext.GetOrAddAsync("SeasonsOrderedDescendingByEndYearWithTablesIncluded",
                GetSeasonsOrderedDescendingByEndYearWithTablesIncludedQuery());

            var doGivenSeasonExist = seasons.SingleOrDefault(season => season.Id == request.SeasonId) != null;
            if (!doGivenSeasonExist)
            {
                var seasonNotExistResponseBody = new AddTableResponse
                {
                    Success = false,
                    Message = "The specified season does not exist."
                };
                return new ServiceActionResult<AddTableResponse>(
                    HttpStatusCode.NotFound, seasonNotExistResponseBody);
            }

            var newTable = mapper.Map<LeagueTable>(request);

            await databaseContext.LeagueTables.AddAsync(newTable);
            await databaseContext.SaveChangesAsync();

            var addedTableDbData = GetLeagueTableWithItsEntriesAndSeasonIncluded(newTable.Id)();
            await cacheContext.AddAsync($"LeagueTableWithItsEntriesAndRelatedSeasonIncluded_{addedTableDbData.Id}", addedTableDbData);
            var updatedSeasonsData = GetSeasonsOrderedDescendingByEndYearWithTablesIncludedQuery()();
            await cacheContext.AddAsync("SeasonsOrderedDescendingByEndYearWithTablesIncluded", updatedSeasonsData);

            var successResponseBody = new AddTableResponse
            {
                Success = true,
                Message = "New table information has been successfully added."
            };
            return new ServiceActionResult<AddTableResponse>(
                HttpStatusCode.Created, successResponseBody);
        }

        public async Task<ServiceActionResult<AddSeasonResponse>> AddSeasonAsync(AddSeasonRequest request)
        {
            var newSeason = mapper.Map<Season>(request);

            await databaseContext.Seasons.AddAsync(newSeason);
            await databaseContext.SaveChangesAsync();

            var updatedSeasonsData = GetSeasonsOrderedDescendingByEndYearWithTablesIncludedQuery()();
            await cacheContext.AddAsync("SeasonsOrderedDescendingByEndYearWithTablesIncluded", updatedSeasonsData);

            var successResponseBody = new AddSeasonResponse
            {
                Success = true,
                Message = "New season information has been successfully added."
            };
            return new ServiceActionResult<AddSeasonResponse>(
                HttpStatusCode.Created, successResponseBody);
        }

        public async Task<ServiceActionResult<DeleteTableResponse>> DeleteTableAsync(int id)
        {
            var table = await cacheContext.GetOrAddAsync($"LeagueTableWithItsEntriesAndRelatedSeasonIncluded_{id}",
                GetLeagueTableWithItsEntriesAndSeasonIncluded(id));
            if (table == null)
            {
                var notFoundResponseBody = new DeleteTableResponse
                {
                    Success = false,
                    Message = "Sorry, there is not given table available."
                };
                return new ServiceActionResult<DeleteTableResponse>(
                    HttpStatusCode.NotFound, notFoundResponseBody);
            }

            databaseContext.LeagueTables.Remove(table);
            await databaseContext.SaveChangesAsync();

            await cacheContext.RemoveAsync($"LeagueTableWithItsEntriesAndRelatedSeasonIncluded_{id}");
            var updatedSeasonsData = GetSeasonsOrderedDescendingByEndYearWithTablesIncludedQuery()();
            await cacheContext.AddAsync("SeasonsOrderedDescendingByEndYearWithTablesIncluded", updatedSeasonsData);

            var successResponseBody = new DeleteTableResponse
            {
                Success = true,
                Message = "The table has been successfully removed."
            };
            return new ServiceActionResult<DeleteTableResponse>(
                HttpStatusCode.OK, successResponseBody);
        }

        public async Task<ServiceActionResult<GetSeasonResponse>> GetSeasonAsync(int id)
        {
            var seasons = await cacheContext.GetOrAddAsync("SeasonsOrderedDescendingByEndYearWithTablesIncluded",
                GetSeasonsOrderedDescendingByEndYearWithTablesIncludedQuery());
            var season = seasons.SingleOrDefault(season => season.Id == id);
            if (season == null)
            {
                var notFoundResponseBody = new GetSeasonResponse
                {
                    Success = false,
                    Message = "Sorry, there is not given season data available."
                };
                return new ServiceActionResult<GetSeasonResponse>(
                    HttpStatusCode.NotFound, notFoundResponseBody);
            }

            var successResponseBody = mapper.Map<GetSeasonResponse>(season);
            successResponseBody.Success = true;

            return new ServiceActionResult<GetSeasonResponse>(
                HttpStatusCode.OK, successResponseBody);
        }

        public async Task<ServiceActionResult<EditSeasonResponse>> EditSeasonAsync(int id, EditSeasonRequest request)
        {
            var seasons = await cacheContext.GetOrAddAsync("SeasonsOrderedDescendingByEndYearWithTablesIncluded",
                GetSeasonsOrderedDescendingByEndYearWithTablesIncludedQuery());
            var season = seasons.SingleOrDefault(season => season.Id == id);
            if (season == null)
            {
                var notFoundResponseBody = new EditSeasonResponse
                {
                    Success = false,
                    Message = "Sorry, there is not given season data available."
                };
                return new ServiceActionResult<EditSeasonResponse>(
                    HttpStatusCode.NotFound, notFoundResponseBody);
            }

            mapper.Map(request, season);
            databaseContext.Seasons.Update(season);
            await databaseContext.SaveChangesAsync();

            var updatedSeasonsData = GetSeasonsOrderedDescendingByEndYearWithTablesIncludedQuery()();
            await cacheContext.AddAsync("SeasonsOrderedDescendingByEndYearWithTablesIncluded", updatedSeasonsData);
            foreach (var table in season.LeaguesTables)
            {
                var updatedTableDbData = GetLeagueTableWithItsEntriesAndSeasonIncluded(table.Id)();
                await cacheContext.AddAsync($"LeagueTableWithItsEntriesAndRelatedSeasonIncluded_{updatedTableDbData.Id}", updatedTableDbData);
            }

            var successResponse = new EditSeasonResponse
            {
                Success = true,
                Message = "The season information has been successfully updated."
            };
            return new ServiceActionResult<EditSeasonResponse>(
                HttpStatusCode.OK, successResponse);
        }

        public async Task<ServiceActionResult<DeleteSeasonResponse>> DeleteSeasonAsync(int id)
        {
            var seasons = await cacheContext.GetOrAddAsync("SeasonsOrderedDescendingByEndYearWithTablesIncluded",
                GetSeasonsOrderedDescendingByEndYearWithTablesIncludedQuery());
            var season = seasons.SingleOrDefault(season => season.Id == id);
            if (season == null)
            {
                var notFoundResponseBody = new DeleteSeasonResponse
                {
                    Success = false,
                    Message = "Sorry, there is not given season available."
                };
                return new ServiceActionResult<DeleteSeasonResponse>(
                    HttpStatusCode.NotFound, notFoundResponseBody);
            }

            databaseContext.Seasons.Remove(season);
            await databaseContext.SaveChangesAsync();

            var updatedSeasonsData = GetSeasonsOrderedDescendingByEndYearWithTablesIncludedQuery()();
            await cacheContext.AddAsync("SeasonsOrderedDescendingByEndYearWithTablesIncluded", updatedSeasonsData);
            foreach (var table in season.LeaguesTables)
            {
                await cacheContext.RemoveAsync($"LeagueTableWithItsEntriesAndRelatedSeasonIncluded_{table.Id}");
            }

            var successResponseBody = new DeleteSeasonResponse
            {
                Success = true,
                Message = "The season and all tables related to it have been successfully removed."
            };
            return new ServiceActionResult<DeleteSeasonResponse>(
                HttpStatusCode.OK, successResponseBody);
        }

        public async Task<ServiceActionResult<GetTableNameResponse>> GetTableNameAsync(int id)
        {
            var table = await cacheContext.GetOrAddAsync($"LeagueTableWithItsEntriesAndRelatedSeasonIncluded_{id}",
                GetLeagueTableWithItsEntriesAndSeasonIncluded(id));
            if (table == null)
            {
                var notFoundResponseBody = new GetTableNameResponse
                {
                    Success = false,
                    Message = "Sorry, there is not given table data available."
                };
                return new ServiceActionResult<GetTableNameResponse>(
                    HttpStatusCode.NotFound, notFoundResponseBody);
            }

            var successResponseBody = new GetTableNameResponse
            {
                Success = true,
                Name = $"{table.Name} - {table.Season.Name}",
            };
            return new ServiceActionResult<GetTableNameResponse>(
                HttpStatusCode.OK, successResponseBody);
        }

        public async Task<ServiceActionResult<EditTableResponse>> EditTableAsync(int id, EditTableRequest request)
        {
            var table = await cacheContext.GetOrAddAsync($"LeagueTableWithItsEntriesAndRelatedSeasonIncluded_{id}",
                GetLeagueTableWithItsEntriesAndSeasonIncluded(id));
            if (table == null)
            {
                var notFoundResponseBody = new EditTableResponse
                {
                    Success = false,
                    Message = "Sorry, there is not given table data available."
                };
                return new ServiceActionResult<EditTableResponse>(
                    HttpStatusCode.NotFound, notFoundResponseBody);
            }

            var entries = GetTableEntriesFromTsv(request.Entries.Split("\n"), id);

            databaseContext.LeagueTableEntries.RemoveRange(table.TableEntries);
            await databaseContext.LeagueTableEntries.AddRangeAsync(entries);
            await databaseContext.SaveChangesAsync();

            var updatedTableDbData = GetLeagueTableWithItsEntriesAndSeasonIncluded(table.Id)();
            await cacheContext.AddAsync($"LeagueTableWithItsEntriesAndRelatedSeasonIncluded_{updatedTableDbData.Id}", updatedTableDbData);

            var successResponseBody = new EditTableResponse
            {
                Success = true,
                Message = "The table entries have been successfully updated."
            };
            return new ServiceActionResult<EditTableResponse>(
                HttpStatusCode.OK, successResponseBody);
        }

        private IEnumerable<LeagueTableEntry> GetTableEntriesFromTsv(IEnumerable<string> tsvLines, int tableId)
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

        private Func<IEnumerable<Season>> GetSeasonsOrderedDescendingByEndYearWithTablesIncludedQuery()
        {
            var query = () =>
            {
                var seasons = databaseContext.Seasons
                    .Include(season => season.LeaguesTables)
                    .OrderByDescending(season => season.EndYear)
                    .ToList();
                return seasons;
            };
            return query;
        }

        private Func<LeagueTable> GetLeagueTableWithItsEntriesAndSeasonIncluded(int tableId)
        {
            var query = () =>
            {
                var table = databaseContext.LeagueTables
                    .Include(table => table.Season)
                    .Include(table => table.TableEntries)
                    .SingleOrDefault(table => table.Id == tableId);
                return table;
            };
            return query;
        }

        public ServiceActionResult<DebugResponse> Debug_ClearAndSeedFootballTables()
        {
            databaseContext.LeagueTableEntries.RemoveRange(databaseContext.LeagueTableEntries);
            databaseContext.LeagueTables.RemoveRange(databaseContext.LeagueTables);
            databaseContext.Seasons.RemoveRange(databaseContext.Seasons);
            databaseContext.SaveChanges();

            var season = new Season { Name = "2020/2021", StartYear = 2020, EndYear = 2021 };
            databaseContext.Seasons.Add(season);
            databaseContext.SaveChanges();

            var table = new LeagueTable { Name = "Ekstraklasa", SeasonId = season.Id, AdditionalInfo = "" };
            databaseContext.LeagueTables.Add(table);
            databaseContext.SaveChanges();

            var entriesData = File.ReadAllLines(@"Data\Samples\Ekstraklasa_2020_2021.txt");
            var entries = GetTableEntriesFromTsv(entriesData, table.Id);
            databaseContext.LeagueTableEntries.AddRange(entries);
            databaseContext.SaveChanges();

            var successResponseBody = new DebugResponse
            {
                Success = true,
                Message = "Done. Please restart the app cache."
            };
            // To restart the app cache, either restart the app (MemoryCacheProvider and LazyCacheProvider)
            // or flush the cache (RedisCacheProvider - run "flushall" command in the Redis instance console)
            return new ServiceActionResult<DebugResponse>(HttpStatusCode.OK, successResponseBody);
        }
    }
}
