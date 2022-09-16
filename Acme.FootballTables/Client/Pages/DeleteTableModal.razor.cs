using Acme.FootballTables.Shared;
using Blazored.Modal;
using Blazored.Modal.Services;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace Acme.FootballTables.Client.Pages
{
    [Authorize]
    public partial class DeleteTableModal : ComponentBase
	{
        [CascadingParameter] 
        public BlazoredModalInstance ModalInstance { get; set; }

        [Parameter]
        public string TableId { get; set; }

        [Inject]
        private IHttpClientFactory HttpClientFactory { get; set; }

        [Inject]
        private IToastService ToastService { get; set; }

        [Inject]
        private NavigationManager NavigationManager { get; set; }

        private async Task DeleteTable()
        {
            var response = await HttpClientFactory.CreateClient("Acme.FootballTables.ServerAPI.Private")
                .DeleteAsync($"FootballTables/DeleteTable/{TableId}");
            var responseBody = await response.Content.ReadFromJsonAsync<DeleteTableResponse>();

            await ModalInstance.CloseAsync();

            ToastService.ShowToast(responseBody.Success ? ToastLevel.Success : ToastLevel.Error, responseBody.Message);

            if (responseBody.Success)
            {
                NavigationManager.NavigateTo("available_tables");
            }
        }
    }
}
