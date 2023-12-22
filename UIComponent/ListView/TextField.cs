using System;
using System.Linq.Expressions;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace Caspian.UI
{
    public partial class TextField<TEntity> where TEntity : class
    {
        [Parameter]
        public Expression<Func<TEntity, string>> Field { get; set; }

        [CascadingParameter(Name = "DataView")]
        public DataView<TEntity> DataView { get; set; }

        [CascadingParameter(Name = "RowData")]
        public RowData<TEntity> RowData { get; set; }

        [Parameter]
        public RenderFragment Template { get; set; }

        [Parameter]
        public RenderFragment EditTemplate { get; set; }

        [Parameter(CaptureUnmatchedValues = true)]
        public IDictionary<string, object> Attributs { get; set; }
    }
}
