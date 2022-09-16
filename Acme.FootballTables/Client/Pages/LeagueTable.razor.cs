using Acme.FootballTables.Shared;
using Blazored.Modal;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using System.Reflection;

namespace Acme.FootballTables.Client.Pages
{
    public partial class LeagueTable : ComponentBase
    {
        [CascadingParameter] 
        public IModalService ModalService { get; set; }

        [Parameter]
        public string Id { get; set; }

        [Inject]
        private IHttpClientFactory HttpClientFactory { get; set; }

        [Inject]
        private NavigationManager NavigationManager { get; set; }

        private GetTableResponse data;


        protected override async Task OnInitializedAsync()
        {
            data = await HttpClientFactory.CreateClient("Acme.FootballTables.ServerAPI.Public")
                .GetFromJsonAsync<GetTableResponse>($"FootballTables/GetTable/{Id}");
        }

        private async Task OpenDeleteTableModal()
        {
            var parameters = new ModalParameters();
            parameters.Add(nameof(DeleteTableModal.TableId), Id);
            ModalService.Show<DeleteTableModal>("Delete table", parameters);
        }

        private async Task NavigateToEditPage()
        {
            NavigationManager.NavigateTo($"edit_table/{Id}");
        }
    }
}
