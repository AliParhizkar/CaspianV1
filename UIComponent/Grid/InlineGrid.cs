using System;
using Caspian.Common;
using Microsoft.JSInterop;
using Caspian.Common.Service;
using System.Threading.Tasks;

namespace Caspian.UI
{
    public partial class DataGrid<TEntity> : DataView<TEntity>, IEnableLoadData, IGridRowSelect where TEntity : class
    {
        public void OnInitializedOperation()
        {
            ContentHeight = 250;
            if (!AutoHide && Inline)
                CreateInsert();
        }

        void OnParameterSetInint()
        {
            if (!HideInsertIcon)
                HideInsertIcon = !AutoHide && Inline;
        }
    }
}
