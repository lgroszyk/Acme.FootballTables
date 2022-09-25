using Acme.FootballTables.Server.Services;
using Acme.FootballTables.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Acme.FootballTables.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class FootballTablesController : ControllerBase
    {
        private readonly IFootballTableService service;

        public FootballTablesController(IFootballTableService service)
        {
            this.service = service;
        }

        [HttpGet("[action]")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAvailableTables()
        {
            var response = await service.GetAvailableTablesAsync();
            return StatusCode((int)response.ResponseCode, response.ResponseBody);
        }

        [HttpGet("[action]")]
        [Authorize]
        public async Task<IActionResult> GetAvailableSeasons()
        {
            var response = await service.GetAvailableSeasonsAsync();
            return StatusCode((int)response.ResponseCode, response.ResponseBody);
        }

        [HttpGet("[action]/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTable(int id)
        {
            var response = await service.GetTableAsync(id);
            return StatusCode((int)response.ResponseCode, response.ResponseBody);
        }

        [HttpGet("[action]/{id}")]
        [Authorize]
        public async Task<IActionResult> GetTableName(int id)
        {
            var response = await service.GetTableNameAsync(id);
            return StatusCode((int)response.ResponseCode, response.ResponseBody);
        }

        [HttpGet("[action]/{id}")]
        [Authorize]
        public async Task<IActionResult> GetSeason(int id)
        {
            var response = await service.GetSeasonAsync(id);
            return StatusCode((int)response.ResponseCode, response.ResponseBody);
        }

        [HttpPost("[action]")]
        [Authorize]
        public async Task<IActionResult> AddTable(AddTableRequest request)
        {
            var response = await service.AddTableAsync(request);
            return StatusCode((int)response.ResponseCode, response.ResponseBody);
        }

        [HttpPost("[action]")]
        [Authorize]
        public async Task<IActionResult> AddSeason(AddSeasonRequest request)
        {
            var response = await service.AddSeasonAsync(request);
            return StatusCode((int)response.ResponseCode, response.ResponseBody);
        }

        [HttpPut("[action]/{id}")]
        [Authorize]
        public async Task<IActionResult> EditTable(int id, EditTableRequest request)
        {
            var response = await service.EditTableAsync(id, request);
            return StatusCode((int)response.ResponseCode, response.ResponseBody);
        }

        [HttpPut("[action]/{id}")]
        [Authorize]
        public async Task<IActionResult> EditSeason(int id, EditSeasonRequest request)
        {
            var response = await service.EditSeasonAsync(id, request);
            return StatusCode((int)response.ResponseCode, response.ResponseBody);
        }

        [HttpDelete("[action]/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteTable(int id)
        {
            var response = await service.DeleteTableAsync(id);
            return StatusCode((int)response.ResponseCode, response.ResponseBody);
        }

        [HttpDelete("[action]/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteSeason(int id)
        {
            var response = await service.DeleteSeasonAsync(id);
            return StatusCode((int)response.ResponseCode, response.ResponseBody);
        }

        [HttpGet("[action]")]
        [AllowAnonymous]
        public IActionResult Debug()
        {
            var response = service.Debug_ClearAndSeedFootballTables();
            return StatusCode((int)response.ResponseCode, response.ResponseBody);
        }
    }
}
