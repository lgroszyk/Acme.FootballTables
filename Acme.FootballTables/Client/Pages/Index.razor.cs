using Acme.FootballTables.Shared;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.Net.Http.Json;

namespace Acme.FootballTables.Client.Pages
{
    public partial class Index : ComponentBase
    {
        [Inject]
        private IHttpClientFactory HttpClientFactory { get; set; }

        [Inject]
        private IToastService ToastService { get; set; }

        private async Task Debug()
        {
            try
            {
                var responseBody = await HttpClientFactory.CreateClient("Acme.FootballTables.ServerAPI.Private")
                    .GetFromJsonAsync<DebugResponse>("FootballTables/Debug");

                ToastService.ShowToast(responseBody.Success ? ToastLevel.Success : ToastLevel.Error, responseBody.Message);
            }
            catch (AccessTokenNotAvailableException exception)
            {
                exception.Redirect();
            }
        }
    }
}
