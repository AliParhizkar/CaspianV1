using Microsoft.AspNetCore.Components;

namespace Caspian.UI
{
    public partial class DataToolsBar<TEntity> where TEntity: class
    {
        [Parameter]
        public bool HideInsertIcon { get; set; }

        [CascadingParameter(Name = "DataView")]
        public DataView<TEntity> DataView { get; set; }

        protected virtual async Task OpenAddForm()
        {
            var entity = Activator.CreateInstance<TEntity>();
            if (DataView.OnInternalUpsert.HasDelegate)
                await DataView.OnInternalUpsert.InvokeAsync(entity);
            if (DataView.OnUpsert.HasDelegate)
                await DataView.OnUpsert.InvokeAsync(entity);
            if (DataView.Inline)
                DataView.CreateInsert();
            if (DataView is ListView<TEntity> listView && listView.UpsertType != UpsertType.Inline)
                listView.OpenPopupWindow();
                
        }
    }
}
