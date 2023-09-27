using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace Caspian.UI
{
    partial class DataGrid<TEntity> : ComponentBase
    {
        [Inject]
        public ProtectedSessionStorage storage { get; set; }

        [Parameter]
        public bool PersistState { get; set; }

        [Parameter]
        public EventCallback<TEntity> SearchChanged { get; set; }

        protected override async Task OnInitializedAsync()
        {
            if (PersistState)
            {
                var name = typeof(TEntity).Name;
                var result = await storage.GetAsync<string>(name);
                if (result.Success)
                {
                    var data = JsonSerializer.Deserialize<GridSPersistStateData<TEntity>>(result.Value);
                    PageNumber = data.PageNumber;
                    Search = data.Search;
                    SelectedRowIndex = data.SelectedRowIndex;
                    if (SearchChanged.HasDelegate)
                        await SearchChanged.InvokeAsync(Search);
                }
            }
            await base.OnInitializedAsync();
        }

        public async Task SetStateGridData()
        {
            if (PersistState)
            {
                var name = typeof(TEntity).Name;
                var data = new GridSPersistStateData<TEntity>
                {
                    PageNumber = PageNumber,
                    Search = Search,
                    SelectedRowIndex = SelectedRowIndex
                };
                var json = JsonSerializer.Serialize(data);
                await storage.SetAsync(name, json);
            }
        }
    }

    internal class GridSPersistStateData<TEntity>
    {
        public int PageNumber { get; set; }

        public TEntity Search { get; set; }

        public int? SelectedRowIndex { get; set; }
    }
}
