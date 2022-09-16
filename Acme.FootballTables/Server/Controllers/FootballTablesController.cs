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
        public IActionResult GetAvailableTables()
        {
            var response = service.GetAvailableTables();
            return StatusCode((int)response.ResponseCode, response.ResponseBody);
        }

        [HttpGet("[action]/{id}")]
        [AllowAnonymous]
        public IActionResult GetTable(int id)
        {
            var response = service.GetTable(id);
            return StatusCode((int)response.ResponseCode, response.ResponseBody);
        }

        [HttpPost("[action]")]
        [Authorize]
        public async Task<IActionResult> AddTable(AddTableRequest request)
        {
            var response = await service.AddTable(request);
            return StatusCode((int)response.ResponseCode, response.ResponseBody);
        }

        [HttpPost("[action]")]
        [Authorize]
        public async Task<IActionResult> AddSeason(AddSeasonRequest request)
        {
            var response = await service.AddSeason(request);
            return StatusCode((int)response.ResponseCode, response.ResponseBody);
        }

        [HttpGet("[action]")]
        [Authorize]
        public IActionResult GetAvailableSeasons()
        {
            var response = service.GetAvailableSeasons();
            return StatusCode((int)response.ResponseCode, response.ResponseBody);
        }

        [HttpDelete("[action]/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteTable(int id)
        {
            var response = await service.DeleteTable(id);
            return StatusCode((int)response.ResponseCode, response.ResponseBody);
        }

        [HttpGet("[action]/{id}")]
        [Authorize]
        public async Task<IActionResult> GetSeason(int id)
        {
            var response = await service.GetSeason(id);
            return StatusCode((int)response.ResponseCode, response.ResponseBody);
        }

        [HttpPut("[action]/{id}")]
        [Authorize]
        public async Task<IActionResult> EditSeason(int id, EditSeasonRequest request)
        {
            var response = await service.EditSeason(id, request);
            return StatusCode((int)response.ResponseCode, response.ResponseBody);
        }

        [HttpDelete("[action]/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteSeason(int id)
        {
            var response = await service.DeleteSeason(id);
            return StatusCode((int)response.ResponseCode, response.ResponseBody);
        }

        [HttpGet("[action]/{id}")]
        [Authorize]
        public IActionResult GetTableName(int id)
        {
            var response = service.GetTableName(id);
            return StatusCode((int)response.ResponseCode, response.ResponseBody);
        }

        [HttpPut("[action]/{id}")]
        [Authorize]
        public async Task<IActionResult> EditTable(int id, EditTableRequest request)
        {
            var response = await service.EditTable(id, request);
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
