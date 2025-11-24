using Caspian.Common;
using System.Diagnostics;
using System.Linq.Expressions;

namespace Caspian.UI
{
    public class ColumnData
    {
        public string Width { get; set; }

        public string Title { get; set; }

        public Expression Expression { get; set; }

        public Expression AggregateExpression { get; set; }

        public SortType? SortType { get; set; }

        public bool Sortable { get; set; }

        public bool Resizable { get; set; }

        public bool DataField { get; set; }

        public string Id { get; set; }

        public bool Hidden { get; set; }
    }
}
