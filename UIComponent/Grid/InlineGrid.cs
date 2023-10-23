
namespace Caspian.UI
{
    public partial class DataGrid<TEntity> : DataView<TEntity>, IEnableLoadData, IGridRowSelect where TEntity : class
    {
        void OnParameterSetInint()
        {
            if (!HideInsertIcon)
                HideInsertIcon = !AutoHide && Inline;
        }
    }
}
