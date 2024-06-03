using System.Linq.Expressions;

namespace Caspian.UI
{
    public class ColumnAppState<TEntity>
    {
        //public IGridButtonCommand<TEntity> Grid { get; set; }

        public Action<Expression> AddToListAction { get; set; }

        public InitializedState InitializedState { get; set; }

        public IList<Expression> List { get; set; } = new List<Expression>();

        public IList<ColumnData> ColumnsData { get; set; } = new List<ColumnData>();
    }
}
