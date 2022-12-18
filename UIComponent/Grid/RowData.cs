namespace Caspian.UI
{
    public class RowData<TEntity>
    {
        public TEntity Data { get; set; }

        public int RowIndex { get; set; } 

        public object DynamicData { get; set; }

        public bool IsEdite { get; set; }
    }
}
