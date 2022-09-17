using Acme.FootballTables.Shared;

namespace Acme.FootballTables.Server.Services
{
    public interface IFootballTableService
    {
        Task<ServiceActionResult<GetAvailableTablesResponse>> GetAvailableTablesAsync();
        Task<ServiceActionResult<GetAvailableSeasonsResponse>> GetAvailableSeasonsAsync();
        Task<ServiceActionResult<GetTableResponse>> GetTableAsync(int id);
        Task<ServiceActionResult<AddTableResponse>> AddTableAsync(AddTableRequest request);
        Task<ServiceActionResult<AddSeasonResponse>> AddSeasonAsync(AddSeasonRequest request);
        Task<ServiceActionResult<DeleteTableResponse>> DeleteTableAsync(int id);
        Task<ServiceActionResult<GetSeasonResponse>> GetSeasonAsync(int id);
        Task<ServiceActionResult<EditSeasonResponse>> EditSeasonAsync(int id, EditSeasonRequest request);
        Task<ServiceActionResult<DeleteSeasonResponse>> DeleteSeasonAsync(int id);
        Task<ServiceActionResult<GetTableNameResponse>> GetTableNameAsync(int id);
        Task<ServiceActionResult<EditTableResponse>> EditTableAsync(int id, EditTableRequest request);
        ServiceActionResult<DebugResponse> Debug_ClearAndSeedFootballTables();
    }
}
