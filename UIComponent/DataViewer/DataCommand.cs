using Caspian.Common;
using Caspian.Common.Extension;
using Microsoft.AspNetCore.Components;

namespace Caspian.UI
{
    public partial class DataCommand<TEntity>: ComponentBase where TEntity: class
    {
        protected string editButtonClassName;
        protected string deleteButtonClassName;
        protected bool disabledDelete;
        protected bool disabledEdit;
        protected Dictionary<string, object> attrs;

        [Parameter]
        public Func<TEntity, bool> DisableEditFunc { get; set; }

        [Parameter]
        public Func<TEntity, bool> DisableDeleteFunc { get; set; }

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
                disabledEdit = DisableEditFunc?.Invoke(RowData.Data) == true;
                if (disabledEdit)
                    editButtonClassName += " t-state-disabled";
                disabledDelete = DisableDeleteFunc?.Invoke(RowData.Data) == true;
                if (disabledDelete)
                    deleteButtonClassName += " t-state-disabled";
            }
            base.OnParametersSet();
        }

        protected virtual async Task OpenForm()
        {
            if (!disabledEdit)
            {
                if (DataView.OnInternalUpsert.HasDelegate)
                    await DataView.OnInternalUpsert.InvokeAsync(RowData.Data);
                if (DataView.OnUpsert.HasDelegate)
                    await DataView.OnUpsert.InvokeAsync(RowData.Data);
                if (DataView.Inline)
                    DataView.SetSelectedEntity(RowData.Data);
            }
        }

        protected async Task DeleteAsync()
        {
            if (!disabledDelete)
            {
                var sholdDeletd = true;
                if (DataView.OnDelete != null)
                    sholdDeletd = await DataView.OnDelete(RowData.Data);
                if (sholdDeletd && DataView.OnInternalDelete.HasDelegate)
                    await DataView.OnInternalDelete.InvokeAsync(RowData.Data);
                if (sholdDeletd && DataView.Batch)
                    await DataView.RemoveAsync(RowData.Data);
            }
        }
    }
}
