using Caspian.Common;
using Microsoft.AspNetCore.Components;

namespace Caspian.UI
{
    public partial class DataCommand<TEntity>: ComponentBase where TEntity: class
    {
        protected string editButtonClassName;
        protected string deleteButtonClassName;
        protected Dictionary<string, object> attrs;

        [Parameter]
        public bool DisableEditButton { get; set; }

        [Parameter]
        public bool DisableDeleteButton { get; set; }

        [CascadingParameter(Name = "RowData")]
        public RowData<TEntity> RowData { get; set; }

        [Inject]
        public CaspianDataService service { get; set; }

        [Parameter]
        public bool HideEdit { get; set; }

        [Parameter]
        public bool HideDelete { get; set; }

        [Parameter]
        public string Style { get; set; }

        [CascadingParameter(Name = "DataView")]
        public DataView<TEntity> DataView { get; set; }

        protected override void OnParametersSet()
        {
            if (!Style.HasValue())
            {
                if (HideEdit || HideDelete)
                    Style = "width:55px;";
                else
                    Style = "width:90px;";
            }
            attrs = new Dictionary<string, object>();
            if (Style.HasValue())
                attrs.Add("style", Style);
            editButtonClassName = "t-grid-edit";
            deleteButtonClassName = "t-grid-delete";
            if (RowData != null)
            {
                if (DisableEditButton)
                    editButtonClassName += " t-state-disabled";
                if (DisableDeleteButton)
                    deleteButtonClassName += " t-state-disabled";
            }
            base.OnParametersSet();
        }

        protected virtual async Task OpenForm()
        {
            if (!DisableEditButton)
            {
                if (DataView.OnInternalUpsert.HasDelegate)
                    await DataView.OnInternalUpsert.InvokeAsync(RowData.Data);
                if (DataView.OnOpen.HasDelegate)
                    await DataView.OnOpen.InvokeAsync(RowData.Data);
                if (DataView.Inline)
                    DataView.SetSelectedEntity(RowData.Data);
            }
        }

        protected async Task DeleteAsync()
        {
            if (!DisableDeleteButton)
            {
                var shouldDeleted = true;
                if (DataView.OnDelete != null)
                    shouldDeleted = await DataView.OnDelete(RowData.Data);
                if (shouldDeleted && DataView.OnInternalDelete.HasDelegate)
                    await DataView.OnInternalDelete.InvokeAsync(RowData.Data);
                if (shouldDeleted && DataView.Batch)
                    await DataView.RemoveAsync(RowData.Data);
            }
        }
    }
}
