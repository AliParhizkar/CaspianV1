using Caspian.Common;
using System.Reflection;
using System.Collections;
using Caspian.Engine.Model;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;
using Caspian.Common.Extension;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace Caspian.Engine.Service
{
    

    internal class TempOrderExpr
    {
        public TempOrderExpr(Expression expr, SortType? sortType)
        {
            Expr = expr;
            if (sortType.HasValue)
                SortType = sortType.Value;
            else
                SortType = SortType.Asc;
        }

        public Expression Expr { get; set; }

        public SortType SortType { get; set; }
    }
}
