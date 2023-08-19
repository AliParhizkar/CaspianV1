﻿using System.Linq.Expressions;

namespace Capian.Dynamicform.Component
{
    public class AutoComplete<TEntity> : InputControl
    {
        public Expression<Func<TEntity, string>> TextExpression { get; set; }
    }
}