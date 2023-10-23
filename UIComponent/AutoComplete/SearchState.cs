using System;

namespace Caspian.UI
{
    public class SearchState
    {
        public Type EntityType { get; set; }

        public object Value { get; set; }

        public IGridRowSelect Grid { get; set; }
    }
}
