using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Caspian.UI
{
    public partial class DataToolsBar<TEntity> where TEntity: class
    {
        [Parameter]
        public bool HideInsertIcon { get; set; }

        [CascadingParameter(Name = "DataView")]
        public DataView<TEntity> DataView { get; set; }

        async Task OpenAddForm()
        {
            var entity = Activator.CreateInstance<TEntity>();
            if (DataView.OnInternalUpsert.HasDelegate)
                await DataView.OnInternalUpsert.InvokeAsync(entity);
            if (DataView.OnUpsert.HasDelegate)
                await DataView.OnUpsert.InvokeAsync(entity);
            if (DataView.Inline)
                DataView.CreateInsert();
        }
    }
}
