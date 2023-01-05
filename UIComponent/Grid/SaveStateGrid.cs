using Newtonsoft.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace Caspian.UI
{
    partial class DataGrid<TEntity> : ComponentBase
    {
        [Inject, JsonIgnore]
        public ProtectedSessionStorage storage { get; set; }

        [Parameter, JsonIgnore]
        public bool PersistState { get; set; }

        [Parameter, JsonIgnore]
        public EventCallback<TEntity> SearchChanged { get; set; }

        protected override async Task OnInitializedAsync()
        {
            if (PersistState)
            {
                var name = typeof(TEntity).Name;
                var result = await storage.GetAsync<string>(name);
                if (result.Success)
                {
                    var data = JsonConvert.DeserializeObject<GridSPersistStateData<TEntity>>(result.Value);
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
                var setting = new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    DefaultValueHandling = DefaultValueHandling.Ignore
                };
                var json = JsonConvert.SerializeObject(data, setting);
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
