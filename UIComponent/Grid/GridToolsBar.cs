using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Caspian.UI
{
    public partial class GridToolsBar<TEntity> where TEntity: class
    {
        [Parameter]
        public bool HideInsertIcon { get; set; }

        [CascadingParameter(Name = "Grid")]
        public DataGrid<TEntity> Grid { get; set; }

        async Task OpenAddForm()
        {
            var entity = Activator.CreateInstance<TEntity>();
            if (Grid.OnInternalUpsert.HasDelegate)
                await Grid.OnInternalUpsert.InvokeAsync(entity);
            if (Grid.OnUpsert.HasDelegate)
                await Grid.OnUpsert.InvokeAsync(entity);
        }
    }
}
