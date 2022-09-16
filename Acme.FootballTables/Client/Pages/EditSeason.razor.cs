using Acme.FootballTables.Shared;
using Blazored.Modal;
using Blazored.Modal.Services;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.Net.Http.Json;

namespace Acme.FootballTables.Client.Pages
{
    [Authorize]
    public partial class EditSeason : ComponentBase
	{
        [CascadingParameter]
        public IModalService ModalService { get; set; }

        [Parameter]
        public string Id { get; set; }

        [Inject]
        private IHttpClientFactory HttpClientFactory { get; set; }

        [Inject]
        private NavigationManager NavigationManager { get; set; }

        [Inject]
        private IToastService ToastService { get; set; }

        private EditSeasonRequest model;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                var season = await HttpClientFactory.CreateClient("Acme.FootballTables.ServerAPI.Private")
                    .GetFromJsonAsync<GetSeasonResponse>($"FootballTables/GetSeason/{Id}");

                if (season.Success)
                {
                    model = new EditSeasonRequest
                    {
                        Name = season.Name,
                        StartYear = season.StartYear,
                        EndYear = season.EndYear
                    };
                }
                else
                {
                    ToastService.ShowError(season.Message);
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
                    .PutAsJsonAsync($"FootballTables/EditSeason/{Id}", model);
                var responseBody = await result.Content.ReadFromJsonAsync<EditSeasonResponse>();

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

        private async Task OpenDeleteSeasonModal()
        {
            var parameters = new ModalParameters();
            parameters.Add(nameof(DeleteSeasonModal.TableId), Id);
            ModalService.Show<DeleteSeasonModal>("Delete season", parameters);

        }
    }
}
