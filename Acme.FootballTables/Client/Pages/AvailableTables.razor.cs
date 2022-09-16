using Acme.FootballTables.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.Net.Http.Json;

namespace Acme.FootballTables.Client.Pages
{
    public partial class AvailableTables : ComponentBase
    {
        [Inject]
        private IHttpClientFactory HttpClientFactory { get; set; }

        private GetAvailableTablesResponse data;

        protected override async Task OnInitializedAsync()
        {
            data = await HttpClientFactory.CreateClient("Acme.FootballTables.ServerAPI.Public")
                .GetFromJsonAsync<GetAvailableTablesResponse>($"FootballTables/GetAvailableTables");
        }

    }
}
