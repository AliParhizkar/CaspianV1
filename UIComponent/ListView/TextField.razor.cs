using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;

namespace Caspian.UI
{
    public partial class TextField<TEntity> where TEntity : class
    {
        [CascadingParameter(Name = "DataView")]
        internal IListViewer<TEntity> DataView { get; set; }

        [CascadingParameter(Name = "RowData")]
        internal RowData<TEntity> RowData { get; set; }

        protected override void OnInitialized()
        {
            DataView?.AddDataField(Field.Body);
            base.OnInitialized();
        }

        [Parameter, EditorRequired]
        public Expression<Func<TEntity, string>> Field { get; set; }

        [Parameter]
        public RenderFragment<string> Template { get; set; }

        [Parameter]
        public RenderFragment EditTemplate { get; set; }

        [Parameter(CaptureUnmatchedValues = true)]
        public IDictionary<string, object> Attributs { get; set; }

        [Parameter]
        public bool DataField { get; set; }
    }
}
