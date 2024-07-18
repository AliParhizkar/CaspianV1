using Microsoft.AspNetCore.Components;

namespace Caspian.UI
{
    public partial class DataToolsBar<TEntity> where TEntity: class
    {
        [Parameter]
        public bool HideInsertIcon { get; set; }

        [Parameter]
        public bool DisableInsertIcon { get; set; }

        [CascadingParameter(Name = "DataView")]
        public DataView<TEntity> DataView { get; set; }

        [Parameter]
        public EventCallback<TEntity> OnInsertButoonClicked { get; set; }

        protected virtual async Task OpenAddForm()
        {
            if (!DisableInsertIcon)
            {
                var entity = Activator.CreateInstance<TEntity>();
                if (DataView.OnInternalUpsert.HasDelegate)
                    await DataView.OnInternalUpsert.InvokeAsync(entity);
                if (DataView.OnUpsert.HasDelegate)
                    await DataView.OnUpsert.InvokeAsync(entity);
                if (DataView.Inline && OnInsertButoonClicked.HasDelegate)
                    await OnInsertButoonClicked.InvokeAsync(entity);
                    //DataView.CreateInsert();
                if (DataView is ListView<TEntity> listView && listView.UpsertType != UpsertType.Inline)
                    listView.OpenPopupWindow();
            }
        }
    }
}
