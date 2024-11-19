
namespace Caspian.UI
{
    public partial class DataGrid<TEntity> : DataView<TEntity>, IEnableLoadData, IGridRowSelect where TEntity : class
    {
        void OnParameterSetInint()
        {
            if (ShowInsertIcon == false)
                ShowInsertIcon = !AutoHide && Inline;
        }
    }
}
