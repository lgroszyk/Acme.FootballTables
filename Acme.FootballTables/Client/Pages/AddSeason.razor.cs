using Acme.FootballTables.Shared;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.Net.Http.Json;

namespace Acme.FootballTables.Client.Pages
{
    [Authorize]
    public partial class AddSeason : ComponentBase
	{
        [Inject]
        private IHttpClientFactory HttpClientFactory { get; set; }

        [Inject]
        private NavigationManager NavigationManager { get; set; }

        [Inject]
        private IToastService ToastService { get; set; }

        private AddSeasonRequest data;

        public AddSeason()
        {
            data = new AddSeasonRequest
            {
                StartYear = DateTime.Now.Year,
                EndYear = DateTime.Now.Year
            };
        }

        private async Task HandleValidSubmit()
        {
            try
            {
                var result = await HttpClientFactory.CreateClient("Acme.FootballTables.ServerAPI.Private")
                    .PostAsJsonAsync("FootballTables/AddSeason", data);
                var responseBody = await result.Content.ReadFromJsonAsync<AddSeasonResponse>();

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
