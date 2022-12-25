using System;
using Caspian.Common;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace Caspian.UI
{
    public partial class GridCommandColumns<TEntity> where TEntity: class
    {
        string editButtonClassName;
        string deleteButtonClassName;
        bool disabledDelete;
        bool disabledEdit;
        Dictionary<string, object> attrs;

        [Parameter]
        public Func<TEntity, bool> DisableEditFunc { get; set; }

        [Parameter]
        public Func<TEntity, bool> DisableDeleteFunc { get; set; }

        [CascadingParameter(Name = "GridRowData")]
        public RowData<TEntity> RowData { get; set; }

        [Parameter]
        public bool HideEdit { get; set; }

        [Parameter]
        public bool HideDelete { get; set; }

        [Parameter]
        public string HeaderTitle { get; set; }

        [Parameter]
        public string HeaderWidth { get; set; }

        [Parameter]
        public string Style { get; set; }

        [CascadingParameter(Name = "Grid")]
        public DataGrid<TEntity> Grid { get; set; }

        protected override void OnParametersSet()
        {
            if (!HeaderTitle.HasValue())
            {
                if (!HideEdit)
                {
                    HeaderTitle = "ویرایش";
                    if (!HideDelete)
                        HeaderTitle += "-";
                }
                if (!HideDelete)
                    HeaderTitle += "حذف";
            }
            if (!HeaderWidth.HasValue())
            {
                if (HideEdit || HideDelete)
                    HeaderWidth = "width:55px;";
                else
                    HeaderWidth = "width:90px;";
            }
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

        async Task OpenForm()
        {
            if (!disabledEdit)
            {
                if (Grid.OnInternalUpsert.HasDelegate)
                    await Grid.OnInternalUpsert.InvokeAsync(RowData.Data);
                if (Grid.OnUpsert.HasDelegate)
                    await Grid.OnUpsert.InvokeAsync(RowData.Data);
                if (Grid.Inline)
                    Grid.SetSelectedEntity(RowData.Data);
            }
        }

        async Task DeleteAsync()
        {
            if (!disabledDelete)
            {
                var sholdDeletd = true;
                if (Grid.OnDelete != null)
                    sholdDeletd = await Grid.OnDelete(RowData.Data);
                if (sholdDeletd && Grid.OnInternalDelete.HasDelegate)
                    await Grid.OnInternalDelete.InvokeAsync(RowData.Data);
                if (sholdDeletd && Grid.Batch)
                    await Grid.RemoveAsync(RowData.Data);
            }
        }
    }
}
