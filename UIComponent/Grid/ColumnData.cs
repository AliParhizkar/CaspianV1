﻿using Caspian.Common;
using System.Linq.Expressions;

namespace Caspian.UI
{
    public class ColumnData
    {
        public string Width { get; set; }

        public string Title { get; set; }

        public LambdaExpression FromExpression { get; set; }

        public LambdaExpression ToExpression { get; set; }

        public Expression Expression { get; set; }

        public Expression AggregateExpression { get; set; }

        public OrderType? OrderType { get; set; }

        public bool Orderable { get; set; }

        public bool Resizeable { get; set; }
    }
}
