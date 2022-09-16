using Acme.FootballTables.Shared;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.Net.Http.Json;

namespace Acme.FootballTables.Client.Pages
{
    [Authorize]
    public partial class EditTable : ComponentBase
	{
        [Parameter]
        public string Id { get; set; }

        [Inject]
        private IHttpClientFactory HttpClientFactory { get; set; }

        [Inject]
        private NavigationManager NavigationManager { get; set; }

        [Inject]
        private IToastService ToastService { get; set; }

        private string tableName;
        private EditTableRequest model = new();

        protected override async Task OnInitializedAsync()
        {
            try
            {
                var getNameResponse = await HttpClientFactory.CreateClient("Acme.FootballTables.ServerAPI.Private")
                    .GetFromJsonAsync<GetTableNameResponse>($"FootballTables/GetTableName/{Id}");

                if (getNameResponse.Success)
                {
                    tableName = getNameResponse.Name;
                }
                else
                {
                    ToastService.ShowError(getNameResponse.Message);
                    NavigationManager.NavigateTo("available_tables");
                }
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
                    .PutAsJsonAsync($"FootballTables/EditTable/{Id}", model);
                var responseBody = await result.Content.ReadFromJsonAsync<EditTableResponse>();

                ToastService.ShowToast(responseBody.Success ? ToastLevel.Success : ToastLevel.Error, responseBody.Message);

                if (responseBody.Success)
                {
                    NavigationManager.NavigateTo($"league_table/{Id}");
                }
            }
            catch (AccessTokenNotAvailableException exception)
            {
                exception.Redirect();
            }
        }
    }
}
