using Acme.FootballTables.Shared;

namespace Acme.FootballTables.Server.Services
{
    public interface IFootballTableService
    {
        ServiceActionResult<GetAvailableTablesResponse> GetAvailableTables();
        ServiceActionResult<GetAvailableSeasonsResponse> GetAvailableSeasons();
        ServiceActionResult<GetTableResponse> GetTable(int id);
        Task<ServiceActionResult<AddTableResponse>> AddTable(AddTableRequest request);
        Task<ServiceActionResult<AddSeasonResponse>> AddSeason(AddSeasonRequest request);
        Task<ServiceActionResult<DeleteTableResponse>> DeleteTable(int id);
        Task<ServiceActionResult<GetSeasonResponse>> GetSeason(int id);
        Task<ServiceActionResult<EditSeasonResponse>> EditSeason(int id, EditSeasonRequest request);
        Task<ServiceActionResult<DeleteSeasonResponse>> DeleteSeason(int id);
        ServiceActionResult<GetTableNameResponse> GetTableName(int id);
        Task<ServiceActionResult<EditTableResponse>> EditTable(int id, EditTableRequest request);
        ServiceActionResult<string> Debug_ClearAndSeedFootballTables();
    }
}
