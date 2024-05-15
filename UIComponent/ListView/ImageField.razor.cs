using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;

namespace Caspian.UI
{
    public partial class ImageField<TEntity> where TEntity : class
    {
        [Parameter]
        public Expression<Func<TEntity, byte[]>> Field { get; set; }

        [CascadingParameter(Name = "DataView")]
        internal IListViewer<TEntity> DataView { get; set; }

        [CascadingParameter(Name = "RowData")]
        public RowData<TEntity> RowData { get; set; }

        [Parameter]
        public RenderFragment Template { get; set; }

        [Parameter]
        public string Style { get; set; }
    }
}
