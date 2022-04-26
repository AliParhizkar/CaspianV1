﻿using System;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;

namespace Caspian.UI
{
    public class SearchState
    {
        public ElementReference SearchForm { get; set; }

        public ElementReference Input { get; set; }

        public Type EntityType { get; set; }

        public object Value { get; set; }

        //public IAutoCompleteValueInitializer AutoComplete { get; set; }

        public IGridRowSelect Grid { get; set; }
    }
}
