using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;

namespace Caspian.UI
{
    public partial class ImageField<TEntity> where TEntity : class
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

        [Parameter]
        public Expression<Func<TEntity, byte[]>> Field { get; set; }

        [Parameter]
        public RenderFragment Template { get; set; }

        [Parameter]
        public string Style { get; set; }
    }
}
