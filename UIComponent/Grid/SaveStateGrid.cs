using System.Text.Json;
using System.Threading.Tasks;
using Caspian.Common;
using Microsoft.AspNetCore.Components;
using UIComponent.Grid;

namespace Caspian.UI
{
    partial class DataGrid<TEntity> : DataView<TEntity> where TEntity : class
    {
        [CascadingParameter]
        public GridStateContext GridStateContext { get; set; }

        [Parameter]
        public bool PersistState { get; set; }

        [Parameter]
        public EventCallback<TEntity> SearchChanged { get; set; }

        protected override async Task OnInitializedAsync()
        {
            if (PersistState)
            {
                if (GridStateContext is null)
                    throw new Exception(@"GridStateContext CascadingParameter can not be null 
                                        when PersistState is true for a grid. you must provide 
                                        a persistance mechanism for GridStateContext persistence methods");

                var name = typeof(TEntity).Name;
                var result = await GridStateContext.GetGridStateAsync(name);
                if (!string.IsNullOrEmpty(result))
                {
                    var data = JsonSerializer.Deserialize<GridSPersistStateData<TEntity>>(result);
                    pageNumber = data.PageNumber;
                    Search = data.Search;
                    SelectedRowIndex = data.SelectedRowIndex;
                    if (SearchChanged.HasDelegate)
                        await SearchChanged.InvokeAsync(Search);
                }
            }
            else
                await Task.Delay(1);
            await base.OnInitializedAsync();
        }

        public async Task SetStateGridData()
        {
            if (PersistState)
            {
                var name = typeof(TEntity).Name;
                var data = new GridSPersistStateData<TEntity>
                {
                    PageNumber = pageNumber,
                    Search = Search,
                    SelectedRowIndex = SelectedRowIndex
                };
                var json = JsonSerializer.Serialize(data);
                await GridStateContext.SaveGridStateAsync(name, json);
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
