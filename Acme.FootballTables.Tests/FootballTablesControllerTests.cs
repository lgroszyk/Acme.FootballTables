using Acme.FootballTables.Shared;
using FluentAssertions;
using NUnit.Framework;
using System.Net;

namespace Acme.FootballTables.Tests
{
    public class FootballTablesControllerTests
    {
        private HttpClient client;

        [SetUp]
        public void Setup()
        {
            var factory = new CustomWebApplicationFactory<Program>();
            client = factory.CreateClient();
        }

        [Test]
        public async Task GivenSampleTablesInDatabase_WhenRequestingAvailableTables_ThenSampleTablesGroupedBySeasonsAreResponded()
        {
            var getAvailableTablesResponse = await client.GetAsync("FootballTables/GetAvailableTables");
            var getAvailableTablesResponseDto = await getAvailableTablesResponse.Content.ReadFromJsonAsync<GetAvailableTablesResponse>();

            getAvailableTablesResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            getAvailableTablesResponseDto.SeasonTablesSets.Should().HaveCount(3);
            getAvailableTablesResponseDto.SeasonTablesSets.Should().Contain(set => set.SeasonName == "2020/2021");
            getAvailableTablesResponseDto.SeasonTablesSets.Should().Contain(set => set.SeasonName == "2019/2020");
            getAvailableTablesResponseDto.SeasonTablesSets.Should().Contain(set => set.SeasonName == "2018/2019");
            foreach (var seasonTablesSet in getAvailableTablesResponseDto.SeasonTablesSets)
            {
                seasonTablesSet.Tables.Should().HaveCount(3);
                seasonTablesSet.Tables.Should().Contain(table => table.Name == "Ekstraklasa");
                seasonTablesSet.Tables.Should().Contain(table => table.Name == "I liga");
                seasonTablesSet.Tables.Should().Contain(table => table.Name == "II liga");
            }
        }

        [Test]
        public async Task GivenSampleSeasonsInDatabase_WhenRequestingAvailableSeasons_ThenSampleSeasonsAreResponded()
        {
            var getAvailableSeasonsResponse = await client.GetAsync("FootballTables/GetAvailableSeasons");
            var getAvailableSeasonsResponseDto = await getAvailableSeasonsResponse.Content.ReadFromJsonAsync<GetAvailableSeasonsResponse>();

            getAvailableSeasonsResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            getAvailableSeasonsResponseDto.Seasons.Should().HaveCount(3);
            getAvailableSeasonsResponseDto.Seasons.Should().Contain(option => option.Name == "2020/2021");
            getAvailableSeasonsResponseDto.Seasons.Should().Contain(option => option.Name == "2019/2020");
            getAvailableSeasonsResponseDto.Seasons.Should().Contain(option => option.Name == "2018/2019");
        }

        [Test]
        public async Task GivenNotExistingTableId_WhenRequestingTable_ThenNotFoundInfoIsResponded()
        {
            var getTableResponse = await client.GetAsync("FootballTables/GetTable/100");
            var getTableResponseDto = await getTableResponse.Content.ReadFromJsonAsync<GetTableResponse>();

            getTableResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
            getTableResponseDto.Success.Should().BeFalse();
            getTableResponseDto.Message.Should().Be("Sorry, there is not given table data available.");
            getTableResponseDto.Name.Should().BeNull();
            getTableResponseDto.Entries.Should().BeNull();
        }

        [Test]
        public async Task GivenExistingTableId_WhenRequestingTable_ThenTableWithItsEntriesIsResponded()
        {
            var getTableResponse = await client.GetAsync("FootballTables/GetTable/1");
            var getTableResponseDto = await getTableResponse.Content.ReadFromJsonAsync<GetTableResponse>();

            getTableResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            getTableResponseDto.Message.Should().BeNull();
            getTableResponseDto.Name.Should().Be("Ekstraklasa - 2020/2021");
            getTableResponseDto.Entries.Should().HaveCount(16);
            
            var expectedFirstEntry = new TableEntry { Position = "1.", Team = "Legia Warszawa", Points = 64, MatchesWon = 19, MatchesDrawn = 7, MatchesLost = 4, MatchesPlayed = 30, GoalDifference = "+24" };
            var expectedLastEntry = new TableEntry { Position = "16.", Team = "Podbeskidzie Bielsko-Bia³a", Points = 25, MatchesWon = 6, MatchesDrawn = 7, MatchesLost = 17, MatchesPlayed = 30, GoalDifference = "-31" };
            getTableResponseDto.Entries.Single(entry => entry.Team == "Legia Warszawa").Should().BeEquivalentTo(expectedFirstEntry);
            getTableResponseDto.Entries.Single(entry => entry.Team == "Podbeskidzie Bielsko-Bia³a").Should().BeEquivalentTo(expectedLastEntry);
        }

        [Test]
        public async Task GivenNotExistingTableId_WhenRequestingTableName_ThenNotFoundInfoIsResponded()
        {
            var getTableNameResponse = await client.GetAsync("FootballTables/GetTableName/100");
            var getTableNameResponseDto = await getTableNameResponse.Content.ReadFromJsonAsync<GetTableNameResponse>();

            getTableNameResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
            getTableNameResponseDto.Success.Should().BeFalse();
            getTableNameResponseDto.Message.Should().Be("Sorry, there is not given table data available.");
            getTableNameResponseDto.Name.Should().BeNull();
        }

        [Test]
        public async Task GivenExistingTableId_WhenRequestingTableName_ThenTableNameIsResponded()
        {
            var getTableNameResponse = await client.GetAsync("FootballTables/GetTableName/1");
            var getTableNameResponseDto = await getTableNameResponse.Content.ReadFromJsonAsync<GetTableNameResponse>();

            getTableNameResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            getTableNameResponseDto.Success.Should().BeTrue();
            getTableNameResponseDto.Message.Should().BeNull();
            getTableNameResponseDto.Name.Should().Be("Ekstraklasa - 2020/2021");
        }

        [Test]
        public async Task GivenNotExistingSeasonId_WhenRequestingSeason_ThenNotFoundInfoIsResponded()
        {
            var getSeasonResponse = await client.GetAsync("FootballTables/GetSeason/100");
            var getSeasonResponseDto = await getSeasonResponse.Content.ReadFromJsonAsync<GetSeasonResponse>();

            getSeasonResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
            getSeasonResponseDto.Success.Should().BeFalse();
            getSeasonResponseDto.Message.Should().Be("Sorry, there is not given season data available.");
            getSeasonResponseDto.Name.Should().BeNull();
            getSeasonResponseDto.StartYear.Should().Be(default);
            getSeasonResponseDto.EndYear.Should().Be(default);
        }

        [Test]
        public async Task GivenExistingSeasonId_WhenRequestingSeason_ThenSeasonResponded()
        {
            var getSeasonResponse = await client.GetAsync("FootballTables/GetSeason/1");
            var getSeasonResponseDto = await getSeasonResponse.Content.ReadFromJsonAsync<GetSeasonResponse>();

            getSeasonResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            getSeasonResponseDto.Success.Should().BeTrue();
            getSeasonResponseDto.Message.Should().BeNull();
            getSeasonResponseDto.Name.Should().Be("2020/2021");
            getSeasonResponseDto.StartYear.Should().Be(2020);
            getSeasonResponseDto.EndYear.Should().Be(2021);
        }

        [Test]
        public async Task GivenSeasonToAddData_WhenRequestingAddingSeason_ThenSeasonAdditionSuccessIsResponded()
        {
            var addSeasonRequestDto = new AddSeasonRequest
            {
                Name = "2021/2022",
                StartYear = 2021,
                EndYear = 2022
            };
            var addSeasonResponse = await client.PostAsJsonAsync("FootballTables/AddSeason", addSeasonRequestDto);
            var addSeasonResponseDto = await addSeasonResponse.Content.ReadFromJsonAsync<AddSeasonResponse>();

            addSeasonResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            addSeasonResponseDto.Success.Should().BeTrue();
            addSeasonResponseDto.Message.Should().Be("New season information has been successfully added.");

            var getAddedSeasonsResponse = await client.GetAsync("FootballTables/GetSeason/4");
            var getAddedSeasonsResponseDto = await getAddedSeasonsResponse.Content.ReadFromJsonAsync<GetSeasonResponse>();
            
            getAddedSeasonsResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            getAddedSeasonsResponseDto.Success.Should().BeTrue();
            getAddedSeasonsResponseDto.Message.Should().BeNull();
            getAddedSeasonsResponseDto.Name.Should().Be("2021/2022");
            getAddedSeasonsResponseDto.StartYear.Should().Be(2021);
            getAddedSeasonsResponseDto.EndYear.Should().Be(2022);
        }

        [Test]
        public async Task GivenTableToAddDataWithInvalidSeasonId_WhenRequestingAddingTable_ThenTableAdditionFailureIsResponded()
        {
            var addTableRequestDto = new AddTableRequest
            {
                Name = "III liga",
                AdditionalInfo = "",
                SeasonId = 4,
            };
            var addTableResponse = await client.PostAsJsonAsync("FootballTables/AddTable", addTableRequestDto);
            var addTableResponseDto = await addTableResponse.Content.ReadFromJsonAsync<AddTableResponse>();

            addTableResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
            addTableResponseDto.Success.Should().BeFalse();
            addTableResponseDto.Message.Should().Be("The specified season does not exist.");
        }

        [Test]
        public async Task GivenValidTableToAddData_WhenRequestingAddingTable_ThenTableAdditionSuccessIsResponded()
        {
            var addTableRequestDto = new AddTableRequest
            {
                Name = "III liga",
                AdditionalInfo = "",
                SeasonId = 1,
            };
            var addTableResponse = await client.PostAsJsonAsync("FootballTables/AddTable", addTableRequestDto);
            var addTableResponseDto = await addTableResponse.Content.ReadFromJsonAsync<AddTableResponse>();

            addTableResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            addTableResponseDto.Success.Should().BeTrue();
            addTableResponseDto.Message.Should().Be("New table information has been successfully added.");

            var getAddedTableResponse = await client.GetAsync("FootballTables/GetTable/10");
            var getAddedTableResponseDto = await getAddedTableResponse.Content.ReadFromJsonAsync<GetTableResponse>();

            getAddedTableResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            getAddedTableResponseDto.Success.Should().BeTrue();
            getAddedTableResponseDto.Message.Should().BeNull();
            getAddedTableResponseDto.Name.Should().Be("III liga - 2020/2021");
            getAddedTableResponseDto.Entries.Should().BeEmpty();
        }

        [Test]
        public async Task GivenNotExistingSeasonIdToEdit_WhenRequestingEditingSeason_ThenSeasonEditFailureIsResponded()
        {
            var editSeasonRequestDto = new EditSeasonRequest
            {
                Name = "2020/2021 (Polska)",
                StartYear = 2020,
                EndYear = 2021,
            };
            var editSeasonResponse = await client.PutAsJsonAsync("FootballTables/EditSeason/4", editSeasonRequestDto);
            var editSeasonResponseDto = await editSeasonResponse.Content.ReadFromJsonAsync<EditSeasonResponse>();

            editSeasonResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
            editSeasonResponseDto.Success.Should().BeFalse();
            editSeasonResponseDto.Message.Should().Be("Sorry, there is not given season data available.");
        }

        [Test]
        public async Task GivenValidSeasonToEditData_WhenRequestingEditingSeason_ThenSeasonEditSuccessIsResponded()
        {
            var editSeasonRequestDto = new EditSeasonRequest
            {
                Name = "2020/2021 (Polska)",
                StartYear = 2020,
                EndYear = 2021,
            };
            var editSeasonResponse = await client.PutAsJsonAsync("FootballTables/EditSeason/1", editSeasonRequestDto);
            var editSeasonResponseDto = await editSeasonResponse.Content.ReadFromJsonAsync<EditSeasonResponse>();

            editSeasonResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            editSeasonResponseDto.Success.Should().BeTrue();
            editSeasonResponseDto.Message.Should().Be("The season information has been successfully updated.");

            var getEditedSeasonsResponse = await client.GetAsync("FootballTables/GetSeason/1");
            var getEditedSeasonsResponseDto = await getEditedSeasonsResponse.Content.ReadFromJsonAsync<GetSeasonResponse>();

            getEditedSeasonsResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            getEditedSeasonsResponseDto.Success.Should().BeTrue();
            getEditedSeasonsResponseDto.Message.Should().BeNull();
            getEditedSeasonsResponseDto.Name.Should().Be("2020/2021 (Polska)");
            getEditedSeasonsResponseDto.StartYear.Should().Be(2020);
            getEditedSeasonsResponseDto.EndYear.Should().Be(2021);

            var editedSeasonTablesIds = new[] { 1, 2, 3 };
            foreach (var editedTableId in editedSeasonTablesIds)
            {
                var getEditedTableResponse = await client.GetAsync($"FootballTables/GetTable/{editedTableId}");
                var getEditedTableResponseDto = await getEditedTableResponse.Content.ReadFromJsonAsync<GetTableResponse>();

                getEditedTableResponse.StatusCode.Should().Be(HttpStatusCode.OK);
                getEditedTableResponseDto.Success.Should().BeTrue();
                getEditedTableResponseDto.Message.Should().BeNull();
                getEditedTableResponseDto.Name.Should().Contain("2020/2021 (Polska)");
            }
        }

        [Test]
        public async Task GivenNotExistingTableIdToEdit_WhenRequestingEditingTable_ThenTableEditFailureIsResponded()
        {
            var editTableRequestDto = new EditTableRequest
            {
                Entries = "dummy"
            };

            var editTableResponse = await client.PutAsJsonAsync("FootballTables/EditTable/10", editTableRequestDto);
            var editTableResponseDto = await editTableResponse.Content.ReadFromJsonAsync<EditTableResponse>();

            editTableResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
            editTableResponseDto.Success.Should().BeFalse();
            editTableResponseDto.Message.Should().Be("Sorry, there is not given table data available.");
        }

        [Test]
        public async Task GivenValidTableToEditData_WhenRequestingEditingTable_ThenTableEditSuccessIsResponded()
        {
            var editTableRequestDto = new EditTableRequest
            {
                Entries = File.ReadAllText(@"Data\Samples\Ekstraklasa_2021_2022.txt")
            };

            var editTableResponse = await client.PutAsJsonAsync("FootballTables/EditTable/1", editTableRequestDto);
            var editTableResponseDto = await editTableResponse.Content.ReadFromJsonAsync<EditTableResponse>();

            editTableResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            editTableResponseDto.Success.Should().BeTrue();
            editTableResponseDto.Message.Should().Be("The table entries have been successfully updated.");

            var getEditedTableResponse = await client.GetAsync("FootballTables/GetTable/1");
            var getEditedTableResponseDto = await getEditedTableResponse.Content.ReadFromJsonAsync<GetTableResponse>();

            getEditedTableResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            getEditedTableResponseDto.Success.Should().BeTrue();
            getEditedTableResponseDto.Message.Should().BeNull();

            var expectedFirstEntry = new TableEntry { Position = "1.", Team = "Lech Poznañ", Points = 74, MatchesWon = 22, MatchesDrawn = 8, MatchesLost = 4, MatchesPlayed = 34, GoalDifference = "+43" };
            var expectedLastEntry = new TableEntry { Position = "18.", Team = "Górnik £êczna", Points = 28, MatchesWon = 6, MatchesDrawn = 10, MatchesLost = 18, MatchesPlayed = 34, GoalDifference = "-31" };
            getEditedTableResponseDto.Entries.Single(entry => entry.Team == "Lech Poznañ").Should().BeEquivalentTo(expectedFirstEntry);
            getEditedTableResponseDto.Entries.Single(entry => entry.Team == "Górnik £êczna").Should().BeEquivalentTo(expectedLastEntry);
        }

        [Test]
        public async Task GivenNotExistingTableIdToDelete_WhenRequestingDeletingTable_ThenTableDeleteFailureIsResponded()
        {
            var deleteTableResponse = await client.DeleteAsync("FootballTables/DeleteTable/10");
            var deleteTableResponseDto = await deleteTableResponse.Content.ReadFromJsonAsync<DeleteTableResponse>();

            deleteTableResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
            deleteTableResponseDto.Success.Should().BeFalse();
            deleteTableResponseDto.Message.Should().Be("Sorry, there is not given table available.");
        }

        [Test]
        public async Task GivenValidTableIdToDelete_WhenRequestingDeletingTable_ThenTableDeleteSuccessIsResponded()
        {
            var deleteTableResponse = await client.DeleteAsync("FootballTables/DeleteTable/1");
            var deleteTableResponseDto = await deleteTableResponse.Content.ReadFromJsonAsync<DeleteTableResponse>();

            deleteTableResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            deleteTableResponseDto.Success.Should().BeTrue();
            deleteTableResponseDto.Message.Should().Be("The table has been successfully removed.");

            var getDeletedTableResponse = await client.GetAsync("FootballTables/GetTable/1");
            var getDeletedTableResponseDto = await getDeletedTableResponse.Content.ReadFromJsonAsync<GetTableResponse>();

            getDeletedTableResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
            getDeletedTableResponseDto.Success.Should().BeFalse();
            getDeletedTableResponseDto.Message.Should().Be("Sorry, there is not given table data available.");
            getDeletedTableResponseDto.Name.Should().BeNull();
            getDeletedTableResponseDto.Entries.Should().BeNull();
        }

        [Test]
        public async Task GivenNotExistingSeasonIdToDelete_WhenRequestingDeletingSeason_ThenSeasonDeleteFailureIsResponded()
        {
            var deleteSeasonResponse = await client.DeleteAsync("FootballTables/DeleteSeason/4");
            var deleteSeasonResponseDto = await deleteSeasonResponse.Content.ReadFromJsonAsync<DeleteSeasonResponse>();

            deleteSeasonResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
            deleteSeasonResponseDto.Success.Should().BeFalse();
            deleteSeasonResponseDto.Message.Should().Be("Sorry, there is not given season available.");
        }

        [Test]
        public async Task GivenValidSeasonIdToDelete_WhenRequestingDeletingSeason_ThenSeasonDeleteSuccessIsResponded()
        {
            var deleteSeasonResponse = await client.DeleteAsync("FootballTables/DeleteSeason/1");
            var deleteSeasonResponseDto = await deleteSeasonResponse.Content.ReadFromJsonAsync<DeleteSeasonResponse>();

            deleteSeasonResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            deleteSeasonResponseDto.Success.Should().BeTrue();
            deleteSeasonResponseDto.Message.Should().Be("The season and all tables related to it have been successfully removed.");

            var getDeletedSeasonResponse = await client.GetAsync("FootballTables/GetSeason/1");
            var getDeletedSeasonResponseDto = await getDeletedSeasonResponse.Content.ReadFromJsonAsync<GetSeasonResponse>();

            getDeletedSeasonResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
            getDeletedSeasonResponseDto.Success.Should().BeFalse();
            getDeletedSeasonResponseDto.Message.Should().Be("Sorry, there is not given season data available.");
            getDeletedSeasonResponseDto.Name.Should().BeNull();
            getDeletedSeasonResponseDto.StartYear.Should().Be(default);

            var deletedSeasonTablesIds = new[] { 1, 2, 3 };
            foreach (var deletedTableId in deletedSeasonTablesIds)
            {
                var getDeletedTableResponse = await client.GetAsync($"FootballTables/GetTable/{deletedTableId}");
                var getDeletedTableResponseDto = await getDeletedTableResponse.Content.ReadFromJsonAsync<GetTableResponse>();

                getDeletedTableResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
                getDeletedTableResponseDto.Success.Should().BeFalse();
                getDeletedTableResponseDto.Message.Should().Be("Sorry, there is not given table data available.");
                getDeletedTableResponseDto.Name.Should().BeNull();
                getDeletedTableResponseDto.Entries.Should().BeNull();
            }
        }
    }
}
