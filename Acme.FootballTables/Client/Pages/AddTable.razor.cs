using Acme.FootballTables.Shared;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.Net.Http.Json;

namespace Acme.FootballTables.Client.Pages
{
    [Authorize]
    public partial class AddTable : ComponentBase
	{
        [Inject]
        private IHttpClientFactory HttpClientFactory { get; set; }

        [Inject]
        private NavigationManager NavigationManager { get; set; }

        [Inject]
        private IToastService ToastService { get; set; }

        private IEnumerable<OptionEntry> seasons;
        private AddTableRequest data = new();

        protected override async Task OnInitializedAsync()
        {
            try
            {
                var seasonsResponse = await HttpClientFactory.CreateClient("Acme.FootballTables.ServerAPI.Private")
                    .GetFromJsonAsync<GetAvailableSeasonsResponse>("FootballTables/GetAvailableSeasons");
                seasons = seasonsResponse.Seasons;
            }
            catch (AccessTokenNotAvailableException exception)
            {
                exception.Redirect();
            }
        }

        private async Task HandleValidSubmit()
        {
            try
            {
                var result = await HttpClientFactory.CreateClient("Acme.FootballTables.ServerAPI.Private")
                    .PostAsJsonAsync("FootballTables/AddTable", data);
                var responseBody = await result.Content.ReadFromJsonAsync<AddTableResponse>();

                ToastService.ShowToast(responseBody.Success ? ToastLevel.Success : ToastLevel.Error, responseBody.Message);

                if (responseBody.Success)
                {
                    NavigationManager.NavigateTo("available_tables");
                }
            }
            catch (AccessTokenNotAvailableException exception)
            {
                exception.Redirect();
            }
        }
    }
}
